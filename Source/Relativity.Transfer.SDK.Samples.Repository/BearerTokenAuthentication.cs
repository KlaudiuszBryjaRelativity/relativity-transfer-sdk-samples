using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.Authentication;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Repository;

[Sample(1, "A bearer token authentication",
    "The sample illustrates how to implement a bearer token authentication in RelativityOne.",
    typeof(BearerTokenAuthentication),
    TransferType.UploadFile)]
internal class BearerTokenAuthentication(
    IConsoleLogger consoleLogger,
    IPathExtension pathExtension,
    IRelativityAuthenticationProviderFactory relativityAuthenticationProviderFactory)
    : ISample
{
    public async Task ExecuteAsync(Configuration configuration, CancellationToken token)
    {
        var clientName = configuration.Common.ClientName;
        var jobId = configuration.Common.JobId;
        var source = string.IsNullOrWhiteSpace(configuration.UploadFile.Source)
            ? new DisposablePath<FilePath>(pathExtension.CreateTemporarySourceFile()).Path
            : new FilePath(configuration.UploadFile.Source);
        var destination = string.IsNullOrWhiteSpace(configuration.UploadFile.Destination)
            ? pathExtension.GetDefaultRemoteDirectoryPathForUpload(configuration.Common)
            : new DirectoryPath(configuration.UploadFile.Destination);
        var authenticationProvider = relativityAuthenticationProviderFactory.Create(configuration.Common);

        // The builder follows the Fluent convention, and more options will be added in the future. The only required component (besides the client name)
        // is the authentication provider - a provided one that utilizes an OAuth-based approach has been provided, but the custom implementation can be created.
        var transferClient = TransferClientBuilder.FullPathWorkflow
            .WithAuthentication(authenticationProvider)
            .WithClientName(clientName)
            .Build();

        consoleLogger.PrintCreatingTransfer(jobId, source, destination);

        var result = await transferClient
            .UploadFileAsync(jobId, source, destination, token)
            .ConfigureAwait(false);

        consoleLogger.PrintTransferResult(result);
    }
}