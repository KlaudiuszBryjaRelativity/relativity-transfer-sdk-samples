using System;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Options;
using Relativity.Transfer.SDK.Interfaces.Options.Policies;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.Authentication;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.ProgressHandler;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Repository.FullPathWorkflow;

[Sample(5, "A retry policy",
    "The sample illustrates the implementation of a retry policy to enhance the resilience of a transfer.",
    typeof(UploadDirectoryWithCustomizedRetryPolicy),
    TransferType.UploadDirectory)]
internal class UploadDirectoryWithCustomizedRetryPolicy(
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

        // The exponential retry policy as well as linear policy are policies that can be used to enhance the resilience of a transfer.
        // The retry policy implementation should be assigned to a transfer options.
        var exponentialRetryPolicyOptions = new UploadDirectoryOptions
        {
            TransferRetryPolicyDefinition = TransferRetryPolicyDefinition.ExponentialPolicy(TimeSpan.FromSeconds(2), 2)
        };

        var result = await transferClient
            .UploadDirectoryAsync(jobId, source, destination, exponentialRetryPolicyOptions, progressHandler, token)
            .ConfigureAwait(false);

        consoleLogger.PrintTransferResult(result);
    }
}