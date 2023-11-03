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
            var overwriteDefaultSetting = false;
            while (true)
            {
                Console.Write("  Directory Path: ");
                var path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = GetOrEnterSetting(SettingNames.DownloadCatalog);
                    overwriteDefaultSetting = true;
                }
                
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"  Directory \"{path}\" does not exist.");
                    continue;
                }
                
                if (overwriteDefaultSetting)
                {
                    ConfigHelper.SetSetting(SettingNames.DownloadCatalog, path);
                }

                var fullpath = Path.Combine(path, TransferJobId.ToString());

                try
                {
                    Directory.CreateDirectory(fullpath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  Error creating directory: " + ex.Message);
                    continue;
                }
                
                
                return new DirectoryPath(fullpath);
            }
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

        public override FilePath EnterSourceFilePathOrTakeDefault()
        {
            var destinationFile = ConfigHelper.GetSettingOrPlaceholder(SettingNames.DefaultSourceFilePath);
            
            Console.WriteLine($"  Provide path to the directory you want to {nameof(TransferDirection)} {DirectionPreposition} RelativityOne:");
            Console.WriteLine($"	 (keep it empty to use default path: \"{destinationFile}\"");
            
            var fileshareRootPath = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
            var fileshareDestinationFolder = GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
            return new FilePath(Path.Combine(fileshareRootPath, fileshareDestinationFolder, destinationFile));
        }
    }
}