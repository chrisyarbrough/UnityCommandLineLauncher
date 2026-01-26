public class PlatformSupportTests
{
	[Theory]
	[InlineData(typeof(MacSupport),
		"/Applications/Unity/Hub/Editor/2022.3.10f1/Unity.app",
		"/Applications/Unity/Hub/Editor/2022.3.10f1")]
	[InlineData(typeof(WindowsSupport),
		@"C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe",
		@"C:\Program Files\Unity\Hub\Editor\6000.0.59f2")]
	public void FindInstallationRootReturnsValidRoot(Type platformSupportType, string editorPath, string expectedRoot)
	{
		PlatformSupport platformSupport = (Activator.CreateInstance(platformSupportType) as PlatformSupport)!;
		string root = platformSupport.FindInstallationRoot(editorPath);
		Assert.Equal(expectedRoot, root);
	}
}