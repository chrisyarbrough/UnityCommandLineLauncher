using System.Runtime.InteropServices;

public class PlatformSupportTests
{
	[Fact_PlatformOSX]
	public void FindInstallationRootReturnsValidRoot()
	{
		PlatformSupport platformSupport = PlatformSupport.Create();
		const string editorPath = "/Applications/Unity/Hub/Editor/2022.3.10f1/Unity.app/Contents/MacOS/Unity";
		string root = platformSupport.FindInstallationRoot(editorPath);
		Assert.Equal("/Applications/Unity/Hub/Editor/2022.3.10f1", root);
	}

	// ReSharper disable once InconsistentNaming
	private sealed class Fact_PlatformOSX : FactAttribute
	{
		public Fact_PlatformOSX()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Skip = "Test only runs on macOS";
			}
		}
	}
}