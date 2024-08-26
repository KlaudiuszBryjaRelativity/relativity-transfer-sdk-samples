using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.DTOs;

namespace Relativity.Transfer.SDK.Samples.Core.Helpers;

internal sealed class PathExtension : IPathExtension
{
	private static readonly Regex RootUncPathRegex =
		new(@"(?<root>[\\]{2}files\d*[.]T\d{3}[.][A-Za-z]{4}\d*.*[\\]T\d{3}[A-Za-z]*)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

	public DirectoryPath GetDefaultRemoteDirectoryPathForUpload(CommonConfiguration common)
	{
		return new DirectoryPath(GetDefaultRemoteDirectoryPathForUploadAsString(common));
	}

	public string GetDefaultRemoteDirectoryPathForUploadAsString(CommonConfiguration common)
	{
		return Path.Combine(common.FileShareRoot, common.FileShareRelativePath, common.JobId.ToString());
	}

	public FilePath CreateTemporarySourceFile()
	{
		return new FilePath(CreateFile("Sdk_Sample1_TemporaryFile.txt"));
	}

	public DirectoryPath EnsureLocalDirectory(string path)
	{
		if (path == null) throw new ArgumentNullException(nameof(path));
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		return new DirectoryPath(path);
	}

	public DirectoryPath GetDefaultRemoteDirectoryPathForDownload(CommonConfiguration common)
	{
		return new DirectoryPath(GetDefaultRemoteDirectoryPathForDownloadAsString(common));
	}

	public string GetDefaultRemoteDirectoryPathForDownloadAsString(CommonConfiguration common)
	{
		return Path.Combine(common.FileShareRoot, common.FileShareRelativePath);
	}

	public DirectoryPath GetDestinationDirectoryPathByFileShareInfo(FileShareInfo fileShareInfo,
		string fileShareRelativePath, Guid jobId)
	{
		var fileShareRootPath = GetFileShareRootPath(fileShareInfo.UncPath);

		return new DirectoryPath(Path.Combine(fileShareRootPath, fileShareRelativePath, jobId.ToString()));
	}

	public string GetFileShareRootPath(string uncPath)
	{
		return RootUncPathRegex.Match(uncPath).Groups["root"].Value;
	}

	public DirectoryPath CreateTemporaryDirectoryWithFiles(Guid jobId)
	{
		return new DirectoryPath(CreateDirectoryWithFiles(jobId.ToString()));
	}

	public DirectoryPath CreateDirectoryWithFiles(string directoryName, params string[] files)
	{
		var path = Path.Combine(EnsureRootDirectoryForFiles(), directoryName);
		CreateSmallFiles(directoryName, files);

		return new DirectoryPath(path);
	}

	private static string CreateDirectoryWithFiles(string directoryName)
	{
		const int smallFiles = 100;
		const int largeFiles = 2;
		const long largeFileSize = 200;

		var path = Path.Combine(EnsureRootDirectoryForFiles(), directoryName);
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		CreateSmallFiles(directoryName,
			Enumerable.Range(0, smallFiles).Select(x => $"Transfer SDK Sample Small file {x}.txt"));
		CreateLargeFiles(directoryName,
			Enumerable.Range(0, largeFiles).Select(x => $"Transfer SDK Sample Large file {x}.txt"), largeFileSize);

		return path;
	}

	private static string EnsureRootDirectoryForFiles()
	{
		var path = Path.Combine(Path.GetTempPath(), "TransferSDK Samples");
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		return path;
	}

	private static void CreateSmallFiles(string directoryName, IEnumerable<string> fileNames)
	{
		var path = Path.Combine(EnsureRootDirectoryForFiles(), directoryName);
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		foreach (var file in fileNames)
		{
			var filePath = Path.Combine(path, file);
			using var sw = File.CreateText(filePath);
			sw.WriteLine($"This is sample temporary file for Transfer SDK: {file}");
		}
	}

	private static void CreateLargeFiles(string directoryName, IEnumerable<string> fileNames, long fileSizeInMiB)
	{
		var path = Path.Combine(EnsureRootDirectoryForFiles(), directoryName);
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		const int blocksPerMiB = 128;
		var data = new byte[8192];
		var rng = new Random((int)DateTime.Now.Ticks);

		// Creates randomly filled large file
		foreach (var file in fileNames)
		{
			var filePath = Path.Combine(path, file);
			using var fs = new FileStream(filePath, FileMode.CreateNew);

			for (var i = 0; i < fileSizeInMiB * blocksPerMiB; i++)
			{
				rng.NextBytes(data);
				fs.Write(data, 0, data.Length);
			}
		}
	}

	private static string CreateFile(string fileName)
	{
		var sampleFile = Path.Combine(Path.GetTempPath(), fileName);
		using var sw = File.CreateText(sampleFile);
		sw.WriteLine("This is sample temporary file for Transfer SDK");

		return sampleFile;
	}
}