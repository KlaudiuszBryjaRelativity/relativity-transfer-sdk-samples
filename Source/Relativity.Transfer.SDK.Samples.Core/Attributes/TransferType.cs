namespace Relativity.Transfer.SDK.Samples.Core.Attributes;

internal enum TransferType
{
	Default,
	UploadDirectory,
	UploadFile,
	UploadItems,
	DownloadDirectory,
	DownloadFile,
	UploadItemsByWorkspaceId,
	UploadDirectoryByWorkspaceId,
	UploadDirectoryBasedOnExistingJob,
	DownloadDirectoryBasedOnExistingJob
}