public class UnityVersionComparerTests
{
	[Fact]
	public void UnityVersionParses()
	{
		UnityVersion expected = new()
		{
			Major = 2021,
			Minor = 3,
			Patch = 1,
			Revision = 2,
		};
		const string versionString = "2021.3.1f2";
		UnityVersion actual = new(versionString);
		Assert.Equal(expected, actual);
		Assert.Equal(versionString, actual.ToString());
	}

	[Fact]
	public void UnityVersionWithoutRevisionParses()
	{
		UnityVersion expected = new()
		{
			Major = 2021,
			Minor = 3,
			Patch = 1,
			Revision = 0,
		};
		const string versionString = "2021.3.1";
		UnityVersion actual = new(versionString);
		Assert.Equal(expected, actual);
		Assert.Equal(versionString, actual.ToString());
	}

	[Fact]
	public void MajorIsSortedCorrectly()
	{
		// Ascending order.
		List<string> expected =
		[
			"4.0.0",
			"5.0.0",
			"2017.0.0",
			"2018.0.0",
			"2019.0.0",
			"2020.0.0",
			"2021.0.0",
			"2022.0.0",
			"2023.0.0",
			"6000.0.0f1",
		];
		List<string> versions =
		[
			"5.0.0",
			"4.0.0",
			"2018.0.0",
			"2019.0.0",
			"2017.0.0",
			"2022.0.0",
			"2020.0.0",
			"6000.0.0f1",
			"2023.0.0",
			"2021.0.0",
		];

		versions.Sort(new UnityVersionComparer());

		Assert.Equal(expected, versions);
	}

	[Fact]
	public void SortNewestFirstReturnsDescendingOrder()
	{
		// Descending order (newest first).
		List<string> expected =
		[
			"6000.0.0f1",
			"2023.1.5f1",
			"2022.3.10f2",
			"2021.3.1f2",
			"2021.3.1",
			"2020.2.0",
			"2019.4.15f1",
			"5.6.0",
			"4.7.2",
		];
		string[] unsorted =
		[
			"2021.3.1f2",
			"4.7.2",
			"2023.1.5f1",
			"2019.4.15f1",
			"6000.0.0f1",
			"2020.2.0",
			"5.6.0",
			"2022.3.10f2",
			"2021.3.1",
		];

		IEnumerable<string> actual = UnityVersion.SortNewestFirst(unsorted);

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ComparisonOperatorsWorkCorrectly()
	{
		// Major version comparison
		Assert.True(new UnityVersion("2023.1.0") > new UnityVersion("2022.1.0"));
		Assert.True(new UnityVersion("2022.1.0") < new UnityVersion("2023.1.0"));

		// Minor version comparison
		Assert.True(new UnityVersion("2021.3.0") > new UnityVersion("2021.2.0"));
		Assert.True(new UnityVersion("2021.2.0") < new UnityVersion("2021.3.0"));

		// Patch version comparison
		Assert.True(new UnityVersion("2021.3.2") > new UnityVersion("2021.3.1"));
		Assert.True(new UnityVersion("2021.3.1") < new UnityVersion("2021.3.2"));

		// Revision comparison
		Assert.True(new UnityVersion("2021.3.1f2") > new UnityVersion("2021.3.1f1"));
		Assert.True(new UnityVersion("2021.3.1f1") < new UnityVersion("2021.3.1f2"));

		// With revision vs without
		Assert.True(new UnityVersion("2021.3.1f2") > new UnityVersion("2021.3.1"));
		Assert.True(new UnityVersion("2021.3.1") < new UnityVersion("2021.3.1f2"));

		// Legacy versions
		Assert.True(new UnityVersion("5.6.0") > new UnityVersion("4.7.2"));
		Assert.True(new UnityVersion("4.7.2") < new UnityVersion("5.6.0"));
	}
}