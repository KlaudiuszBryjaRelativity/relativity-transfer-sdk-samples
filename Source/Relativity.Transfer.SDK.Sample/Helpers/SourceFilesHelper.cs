namespace Relativity.Transfer.SDK.Sample.Helpers
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;

	internal static class SourceFilesHelper
	{
		internal static string CreateFile(string fileName)
		{
			string sampleFile = Path.Combine(Path.GetTempPath(), fileName);
			using (StreamWriter sw = File.CreateText(sampleFile))
			{
				sw.WriteLine("This is sample temporary file for Transfer SDK");
			}
			return sampleFile;
		}

		internal static string CreateDirectoryWithFiles(string directoryName, int smallFiles, int largeFiles, long largeFileSize)
		{
			if (smallFiles < 0 || largeFileSize < 0 || largeFileSize < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			string rootPath = Path.Combine(GetRootSamplesFolder(), directoryName);
			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}

			CreateSmallFiles(directoryName, Enumerable.Range(0, smallFiles).Select(x => $"Transfer SDK Sample Small file {x}"));
			CreateLargeFiles(directoryName, Enumerable.Range(0, largeFiles).Select(x => $"Transfer SDK Sample Large file {x}"), largeFileSize);

			return rootPath;
		}

		internal static string CreateDirectoryWithFiles(string directoryName, params string[] files)
		{
			string rootPath = Path.Combine(GetRootSamplesFolder(), directoryName);
			CreateSmallFiles(directoryName, files);
			return rootPath;
		}

		internal static IEnumerable<string> CreateSmallFiles(string relativePath, IEnumerable<string> files)
		{
			string rootPath = Path.Combine(GetRootSamplesFolder(), relativePath);
			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}
			List<string> fullPaths = new List<string>();
			foreach (var file in files)
			{
				string filePath = Path.Combine(rootPath, file);
				using (StreamWriter sw = File.CreateText(filePath))
				{
					sw.WriteLine($"This is sample temporary file for Transfer SDK: {file}");
				}
				fullPaths.Add(filePath);
			}
			return fullPaths;
		}

		internal static IEnumerable<string> CreateLargeFiles(string relativePath, IEnumerable<string> files, long size)
		{
			string rootPath = Path.Combine(GetRootSamplesFolder(), relativePath);
			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}
			List<string> fullPaths = new List<string>();
			foreach (var file in files)
			{
				string filePath = Path.Combine(rootPath, file);
				FileStream fs = new FileStream(filePath, FileMode.CreateNew);
				fs.Seek(size, SeekOrigin.Begin);
				fs.WriteByte(0);
				fs.Close();
				fullPaths.Add(filePath);
			}
			return fullPaths;
		}

		internal static void ClearTransferSdkTemporaryData()
		{
			string rootPath = GetRootSamplesFolder();
			Directory.Delete(rootPath, true);
		}

		private static string GetRootSamplesFolder()
		{
			string rootPath = Path.Combine(Path.GetTempPath(), "TransferSDK Samples");
			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}
			return rootPath;
		}

	}
}
