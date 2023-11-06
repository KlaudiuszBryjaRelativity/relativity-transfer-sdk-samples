namespace Relativity.Transfer.SDK.Sample.Helpers
{
    internal interface IConfigProvider
    {
        string ClientName { get; set; }
        string RelativityOneInstanceUrl { get; set; }
        string RelativityOneFileshareRoot { get; set; }
        string FileshareRelativeDestinationPath { get; set; }
        string DefaultSourceFilePath { get; set; }
        string DefaultSourceDirectoryPath { get; set; }
        string DownloadCatalog { get; set; }
        string ClientSecret { get; set; }
        string ClientOAuth2Id { get; set; }
    }
}

