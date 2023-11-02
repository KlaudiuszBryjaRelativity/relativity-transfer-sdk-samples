using System;
using System.IO;
using Relativity.Transfer.SDK.Interfaces.Paths;

namespace Relativity.Transfer.SDK.Sample.Helpers
{
    internal class DownloadConsoleHelper : ConsoleHelper
    {
        public DownloadConsoleHelper(ConfigHelper configHelper) : base(configHelper)
        { }

        protected override TransferDirection TransferDirection => TransferDirection.Download;

        public override DirectoryPath GetDestinationDirectoryPath()
        {
            var downloadCatalog = ConfigHelper.GetSettingOrPlaceholder(SettingNames.DownloadCatalog);

            if (!Directory.Exists(downloadCatalog))
            {
                //Here throw exception and ask for new path
            }

            var fullpath = Path.Combine(downloadCatalog, TransferJobId.ToString());

            try
            {
                Directory.CreateDirectory(fullpath);
            }
            catch (Exception ex)
            {
                //Handling here
            }

            return new DirectoryPath(fullpath);
        }

        public override DirectoryPath EnterSourceDirectoryPathOrTakeDefault()
        {
            var destinationFolder = ConfigHelper.GetSettingOrPlaceholder(SettingNames.DefaultSourceDirectoryPath);
            
            Console.WriteLine($"  Provide path to the directory you want to {nameof(TransferDirection)} {DirectionPreposition} RelativityOne:");
            Console.WriteLine($"	 (keep it empty to use default path: \"{destinationFolder}\"");
            
            var fileshareRootPath = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
            var fileshareDestinationFolder = GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
            return new DirectoryPath(Path.Combine(fileshareRootPath, fileshareDestinationFolder, destinationFolder));
        }
    }
}