using System;
using System.IO;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Sample.Samples;

namespace Relativity.Transfer.SDK.Sample.Helpers
{
    internal class DownloadConsoleHelper : IConsoleHelper
    {
        private readonly IConsoleHelper _consoleHelper;
        private readonly IConfigProvider _configProvider;

        public DownloadConsoleHelper(IConsoleHelper consoleHelper, IConfigProvider configProvider)
        {
            _consoleHelper = consoleHelper;
            _configProvider = configProvider;
        }

        public void RegisterSample(char key, SampleBase sample)
        {
            _consoleHelper.RegisterSample(key, sample);
        }

        public bool InitStartupSettings()
        {
            return _consoleHelper.InitStartupSettings();
        }

        public async Task RunMenuAsync()
        {
            await _consoleHelper.RunMenuAsync().ConfigureAwait(false);
        }

        public string GetOrEnterSetting(string settingName, bool printValueIfAlreadySet = true)
        {
            return _consoleHelper.GetOrEnterSetting(settingName, printValueIfAlreadySet);
        }

        public string EnterUntilValid(string askingtext, string validationPattern)
        {
            return _consoleHelper.EnterUntilValid(askingtext, validationPattern);
        }

        public DirectoryPath GetDestinationDirectoryPath(string transferJobId)
        {
            var overwriteDefaultSetting = false;
            while (true)
            {
                var defaultPath = GetOrEnterSetting(SettingNames.DownloadDirectory);
                Console.Write($"  Provide path to where transfer should download or leave empty for default '{defaultPath}' : ");
                var path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = defaultPath;
                    overwriteDefaultSetting = true;
                }
                
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"  Directory \"{path}\" does not exist.");
                    continue;
                }
                
                if (overwriteDefaultSetting)
                {
                    _configProvider.DownloadDirectory = path;
                }

                var fullpath = Path.Combine(path, transferJobId);

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

        public DirectoryPath EnterSourceDirectoryPathOrTakeDefault()
        {
            var sourceDirectoryPath = _configProvider.DefaultSourceDirectoryPath;
            
            Console.WriteLine($"  Provide path to the directory you want to download from RelativityOne:");
            Console.WriteLine($"	 (keep it empty to use default path: \"{sourceDirectoryPath}\"");
            
            var overwriteDefaultSetting = false;

            Console.Write("  Directory Path: ");
            var path = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(path))
            {
                sourceDirectoryPath = path;
                overwriteDefaultSetting = true;
            }

            if (overwriteDefaultSetting)
            {
                _configProvider.DefaultSourceFilePath = sourceDirectoryPath;
            }
            
            var fileshareRootPath = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
            var fileshareDestinationFolder = GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
            return new DirectoryPath(Path.Combine(fileshareRootPath, fileshareDestinationFolder, sourceDirectoryPath));
        }

        public FilePath EnterSourceFilePathOrTakeDefault()
        {
            var sourceFilePath = _configProvider.DefaultSourceFilePath;
            
            Console.WriteLine($"  Provide path to the directory you want to download from RelativityOne:");
            Console.WriteLine($"	 (keep it empty to use default path: \"{sourceFilePath}\"");
            
            var overwriteDefaultSetting = false;

            Console.Write("  File Path: ");
            var path = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(path))
            {
                sourceFilePath = path;
                overwriteDefaultSetting = true;
            }

            if (overwriteDefaultSetting)
            {
                _configProvider.DefaultSourceFilePath = sourceFilePath;
            }

            var fileshareRootPath = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
            var fileshareDestinationFolder = GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
            return new FilePath(Path.Combine(fileshareRootPath, fileshareDestinationFolder, sourceFilePath));
        }
    }
}