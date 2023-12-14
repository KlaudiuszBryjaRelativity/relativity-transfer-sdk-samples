namespace Relativity.Transfer.SDK.Sample.Configuration;

internal record Configuration(
	CommonConfiguration Common,
	SourceAndDestinationConfiguration UploadFile,
	SourceAndDestinationConfiguration UploadDirectory,
	SourceAndDestinationConfiguration DownloadFile,
	SourceAndDestinationConfiguration DownloadDirectory,
	SourceAndWorkspaceIdConfiguration UploadDirectoryByWorkspaceId)
{
	public void Deconstruct(out CommonConfiguration common, out SourceAndDestinationConfiguration uploadFile,
		out SourceAndDestinationConfiguration uploadDirectory, out SourceAndDestinationConfiguration downloadFile,
		out SourceAndDestinationConfiguration downloadDirectory,
		out SourceAndWorkspaceIdConfiguration uploadDirectoryByWorkspaceId)
	{
		common = Common;
		uploadFile = UploadFile;
		uploadDirectory = UploadDirectory;
		downloadFile = DownloadFile;
		downloadDirectory = DownloadDirectory;
		uploadDirectoryByWorkspaceId = UploadDirectoryByWorkspaceId;
	}

	internal static Configuration ForUploadDirectory(CommonConfiguration common,
		SourceAndDestinationConfiguration uploadDirectory)
	{
		return new Configuration(common,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			uploadDirectory,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndWorkspaceIdConfiguration(string.Empty, -1));
	}

	internal static Configuration ForUploadFile(CommonConfiguration common,
		SourceAndDestinationConfiguration uploadFile)
	{
		return new Configuration(common,
			uploadFile,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndWorkspaceIdConfiguration(string.Empty, -1));
	}

	internal static Configuration ForDownloadDirectory(CommonConfiguration common,
		SourceAndDestinationConfiguration downloadDirectory)
	{
		return new Configuration(common,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			downloadDirectory,
			new SourceAndWorkspaceIdConfiguration(string.Empty, -1));
	}

	internal static Configuration ForDownloadFile(CommonConfiguration common,
		SourceAndDestinationConfiguration downloadFile)
	{
		return new Configuration(common,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			downloadFile,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndWorkspaceIdConfiguration(string.Empty, -1));
	}

	internal static Configuration ForUploadDirectoryByWorkspaceId(CommonConfiguration common,
		SourceAndWorkspaceIdConfiguration uploadDirectory)
	{
		return new Configuration(common,
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			new SourceAndDestinationConfiguration(string.Empty, string.Empty),
			uploadDirectory);
	}
}