using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.Authentication;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.ProgressHandler;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Repository.FullPathWorkflow;

[Sample(4, 
    "Upload items",
    "The sample illustrates how to implement files upload to a RelativityOne file share. ", 
    typeof(UploadFiles), 
    TransferType.UploadFiles)]
internal class UploadFiles : ISample
{
    private readonly IConsoleLogger _consoleLogger;
    private readonly IPathExtension _pathExtension;
    private readonly IRelativityAuthenticationProviderFactory _relativityAuthenticationProviderFactory;
    private readonly IProgressHandlerFactory _progressHandlerFactory;

    public UploadFiles(IConsoleLogger consoleLogger, IPathExtension pathExtension, IRelativityAuthenticationProviderFactory relativityAuthenticationProviderFactory, IProgressHandlerFactory progressHandlerFactory)
    {
        _consoleLogger = consoleLogger;
        _pathExtension = pathExtension;
        _relativityAuthenticationProviderFactory = relativityAuthenticationProviderFactory;
        _progressHandlerFactory = progressHandlerFactory;
    }
    
    public async Task ExecuteAsync(Configuration configuration, CancellationToken token)
    {
        var clientName = configuration.Common.ClientName;
        var jobId = configuration.Common.JobId;
        var source = new FilePath(configuration.UploadFile.Source);
        var destination = string.IsNullOrWhiteSpace(configuration.UploadFile.Destination)
            ? _pathExtension.GetDefaultRemoteDirectoryPathForUpload(configuration.Common)
            : new DirectoryPath(configuration.UploadFile.Destination);
        var authenticationProvider = _relativityAuthenticationProviderFactory.Create(configuration.Common);
        var progressHandler = _progressHandlerFactory.Create();
        
        var transferClient = TransferClientBuilder.FullPathWorkflow
            .WithAuthentication(authenticationProvider)
            .WithClientName(clientName)
            .WithStagingExplorerContext()
            .Build();
        
        _consoleLogger.PrintCreatingTransfer(jobId, source, destination);
        
        try
        {
            var sources = GetTransferredEntities(configuration.UploadFile.Source);
            var result = await transferClient
                .UploadItemsAsync(jobId, sources, destination, progressHandler, token)
                .ConfigureAwait(false);
            
            _consoleLogger.PrintTransferResult(result);
        }
        catch (Exception e)
        {
            _consoleLogger.PrintError(e);
        }
    }

    private async IAsyncEnumerable<TransferEntity> GetTransferredEntities(string filePath)
    {
        const int maxSourceFiles = 5;
        using var sr = new StreamReader(filePath);

        for (var i = 0; i < maxSourceFiles && await sr.ReadLineAsync() is { } line; i++)
        {
            var paths = line.Split(';');
            if (paths.Length != 2)
            {
                _consoleLogger.Info($"[red]Invalid parameters in {i} line");
                continue;
            }

            var transferEntity = new TransferEntity(new FilePath(paths[0]), new FilePath(paths[1]));
            
            yield return transferEntity;
        }
    }
}