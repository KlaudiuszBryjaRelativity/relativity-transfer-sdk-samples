using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Sample.Attributes;
using Relativity.Transfer.SDK.Sample.Authentication;
using Relativity.Transfer.SDK.Sample.Helpers;
using Relativity.Transfer.SDK.Sample.ProgressHandler;

namespace Relativity.Transfer.SDK.Sample.Samples;

[Sample(4, "Upload a directory",
	"The sample illustrates how to implement a directory upload to a RelativityOne file share.",
	typeof(UploadDirectory),
	TransferType.UploadDirectory)]
internal class UploadDirectory(
	IConsoleLogger consoleLogger,
	IPathExtension pathExtension,
	IRelativityAuthenticationProviderFactory relativityAuthenticationProviderFactory,
	IProgressHandlerFactory progressHandlerFactory)
	: ISample
{
	public async Task ExecuteAsync(Configuration.Configuration configuration, CancellationToken token)
	{
		var clientName = configuration.Common.ClientName;
		var jobId = configuration.Common.JobId;
		var source = new DirectoryPath(configuration.UploadDirectory.Source);
		var destination = string.IsNullOrWhiteSpace(configuration.UploadDirectory.Destination)
			? pathExtension.GetDefaultRemoteDirectoryPathForUpload(configuration.Common)
			: new DirectoryPath(configuration.UploadDirectory.Destination);
		var authenticationProvider = relativityAuthenticationProviderFactory.Create(configuration.Common);
		var progressHandler = progressHandlerFactory.Create();

		// The builder follows the Fluent convention, and more options will be added in the future. The only required component (besides the client name)
		// is the authentication provider - a provided one that utilizes an OAuth-based approach has been provided, but the custom implementation can be created.
		var transferClient = TransferClientBuilder.FullPathWorkflow
			.WithAuthentication(authenticationProvider)
			.WithClientName(clientName)
			.Build();

		consoleLogger.PrintCreatingTransfer(jobId, source, destination);

		var result = await transferClient
			.UploadDirectoryAsync(jobId, source, destination, progressHandler, token)
			.ConfigureAwait(false);

		consoleLogger.PrintTransferResult(result);
	}
}