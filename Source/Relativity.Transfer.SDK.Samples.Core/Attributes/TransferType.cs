namespace Relativity.Transfer.SDK.Samples.Core.Attributes;

internal enum TransferType
{
	Default,
	UploadDirectory,
	UploadFile,
	UploadItems,
	DownloadDirectory,
	DownloadFile,
	UploadDirectoryByWorkspaceId,
	UploadDirectoryBasedOnExistingJob,
	DownloadDirectoryBasedOnExistingJob
}