public class PlatformSupportTests
{
	[Fact]
	public void FindInstallationRootReturnsValidRoot()
	{
		PlatformSupport platformSupport = PlatformSupport.Create();
		const string editorPath = "/Applications/Unity/Hub/Editor/2022.3.10f1/Unity.app/Contents/MacOS/Unity";
		string root = platformSupport.FindInstallationRoot(editorPath);
		Assert.Equal("/Applications/Unity/Hub/Editor/2022.3.10f1", root);
	}
}