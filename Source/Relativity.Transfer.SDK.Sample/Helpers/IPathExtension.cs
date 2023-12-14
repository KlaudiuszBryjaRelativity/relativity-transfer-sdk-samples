using System;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Sample.Configuration;

namespace Relativity.Transfer.SDK.Sample.Helpers;

internal interface IPathExtension
{
	DirectoryPath GetDefaultRemoteDirectoryPathForUpload(CommonConfiguration common);
	string GetDefaultRemoteDirectoryPathForUploadAsString(CommonConfiguration common);
	FilePath CreateTemporarySourceFile();
	DirectoryPath EnsureLocalDirectory(string path);
	DirectoryPath GetDefaultRemoteDirectoryPathForDownload(CommonConfiguration common);
	string GetDefaultRemoteDirectoryPathForDownloadAsString(CommonConfiguration common);

	DirectoryPath GetDestinationDirectoryPathByFileShareInfo(string fileShareRootPath, string fileShareRelativePath,
		Guid jobId);

	DirectoryPath CreateTemporaryDirectoryWithFiles(Guid jobId);
	DirectoryPath CreateDirectoryWithFiles(string directoryName, params string[] files);
}