namespace Relativity.Transfer.SDK.Samples.Core.Attributes;

internal enum TransferType
{
	Default,
	UploadDirectory,
	UploadFile,
	DownloadDirectory,
	DownloadFile,
	UploadDirectoryByWorkspaceId,
	UploadDirectoryBasedOnExistingJob,
	DownloadDirectoryBasedOnExistingJob
}