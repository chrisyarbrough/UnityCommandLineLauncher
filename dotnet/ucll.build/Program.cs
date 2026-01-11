using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

(string runtime, string binaryExtension, Action<string, Func<string, Stream>> compressionMethod)[] targets =
[
	("osx-arm64", string.Empty, CompressTar),
	("osx-x64", string.Empty, CompressTar),
	("linux-arm64", string.Empty, CompressTar),
	("linux-x64", string.Empty, CompressTar),
	("win-arm64", ".exe", CompressZip),
	("win-x64", ".exe", CompressZip),
];

// The source project.
string ucllDirectory = Path.Combine(new DirectoryInfo(AppContext.BaseDirectory).Parent!.Parent!.FullName, "ucll");

// Where the compressed release artifacts are stored.
string artifactsDirectory = PathUtil.PrepareOutputDirectory("artifacts");

List<string> artifactFiles = [];

foreach ((string runtime, string binaryExt, var compressionMethod) in targets)
{
	string publishDirectory = Publish(runtime);

	string sourceFile = Path.Combine(publishDirectory, "ucll" + binaryExt);
	compressionMethod.Invoke(sourceFile, ext =>
	{
		string outputFileWithoutExt = Path.Combine(artifactsDirectory, $"ucll-{runtime}");
		string outputFile = outputFileWithoutExt + ext;
		Console.WriteLine($"Created {outputFile}");
		artifactFiles.Add(outputFile);
		return File.Create(outputFile);
	});
	Console.WriteLine();
}

// Generate SHA256SUMS file
string sha256SumsPath = Path.Combine(artifactsDirectory, "SHA256SUMS");
using (StreamWriter writer = new(sha256SumsPath))
{
	foreach (string artifactFile in artifactFiles)
	{
		string hash = ComputeSHA256(artifactFile);
		string fileName = Path.GetFileName(artifactFile);
		writer.WriteLine($"{hash}  {fileName}");
		Console.WriteLine($"SHA256 ({fileName}): {hash}");
	}
}
Console.WriteLine($"Created {sha256SumsPath}");
Console.WriteLine();

SignWithGPG(sha256SumsPath);
File.Delete(sha256SumsPath);

return 0;

string Publish(string runtime)
{
	ProcessUtil.Run("dotnet", $"publish --runtime {runtime}", ucllDirectory);
	return Path.Combine(ucllDirectory, "bin", runtime, "publish");
}

static void CompressTar(string sourceFile, Func<string, Stream> streamFactory)
{
	using Stream stream = streamFactory.Invoke(".tar.gz");
	using GZipStream gzipStream = new(stream, CompressionMode.Compress);
	using TarWriter tarWriter = new(gzipStream);
	tarWriter.WriteEntry(sourceFile, "ucll");
}

static void CompressZip(string sourceFile, Func<string, Stream> streamFactory)
{
	using Stream stream = streamFactory.Invoke(".zip");
	using ZipArchive archive = new(stream, ZipArchiveMode.Create);
	archive.CreateEntryFromFile(sourceFile, "ucll.exe", CompressionLevel.Optimal);
}

static string ComputeSHA256(string filePath)
{
	using FileStream stream = File.OpenRead(filePath);
	byte[] hashBytes = SHA256.HashData(stream);
	StringBuilder sb = new(hashBytes.Length * 2);
	foreach (byte b in hashBytes)
		sb.Append(b.ToString("x2"));
	return sb.ToString();
}

static void SignWithGPG(string filePath)
{
	string outputPath = filePath + ".asc";

	Process process = ProcessUtil.Run("gpg", $"--clearsign --output \"{outputPath}\" \"{filePath}\"");

	if (process.ExitCode == 0)
	{
		Console.WriteLine($"Signed with GPG: {outputPath}");
	}
	else
	{
		Console.Error.WriteLine("Signing failed, aborting publish.");
		Environment.Exit(process.ExitCode);
	}
}