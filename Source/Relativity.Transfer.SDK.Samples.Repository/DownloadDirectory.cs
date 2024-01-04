using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.Authentication;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.ProgressHandler;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Repository;

[Sample(9, "Download a directory",
    "The sample illustrates how to implement a directory download from a RelativityOne file share.",
    typeof(DownloadDirectory),
    TransferType.DownloadDirectory)]
internal class DownloadDirectory(
    IConsoleLogger consoleLogger,
    IPathExtension pathExtension,
    IRelativityAuthenticationProviderFactory relativityAuthenticationProviderFactory,
    IProgressHandlerFactory progressHandlerFactory)
    : ISample
{
    public async Task ExecuteAsync(Configuration configuration, CancellationToken token)
    {
        var clientName = configuration.Common.ClientName;
        var jobId = configuration.Common.JobId;
        var source = string.IsNullOrWhiteSpace(configuration.DownloadDirectory.Source)
            ? pathExtension.GetDefaultRemoteDirectoryPathForDownload(configuration.Common)
            : new DirectoryPath(configuration.DownloadDirectory.Source);
        var destination = pathExtension.EnsureLocalDirectory(configuration.DownloadDirectory.Destination);
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
            .DownloadDirectoryAsync(jobId, source, destination, progressHandler, token)
            .ConfigureAwait(false);

        consoleLogger.PrintTransferResult(result);
    }
}