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
}