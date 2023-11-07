namespace Relativity.Transfer.SDK.Sample.Helpers
{
    internal class ConfigProvider : IConfigProvider
    {
        private readonly ConfigHelper _configHelper;

        public ConfigProvider(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public string ClientName
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.ClientName);
            set => _configHelper.SetSetting(SettingNames.ClientName, value);
        }

        public string RelativityOneInstanceUrl
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.RelativityOneInstanceUrl);
            set => _configHelper.SetSetting(SettingNames.RelativityOneInstanceUrl, value);
        }

        public string RelativityOneFileshareRoot
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.RelativityOneFileshareRoot);
            set => _configHelper.SetSetting(SettingNames.RelativityOneFileshareRoot, value);
        }

        public string FileshareRelativeDestinationPath
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.FileshareRelativeDestinationPath);
            set => _configHelper.SetSetting(SettingNames.FileshareRelativeDestinationPath, value);
        }

        public string DefaultSourceFilePath
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.DefaultSourceFilePath);
            set => _configHelper.SetSetting(SettingNames.DefaultSourceFilePath, value);
        }

        public string DefaultSourceDirectoryPath
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.DefaultSourceDirectoryPath);
            set => _configHelper.SetSetting(SettingNames.DefaultSourceDirectoryPath, value);
        }

        public string DownloadCatalog
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.DownloadDirectory);
            set => _configHelper.SetSetting(SettingNames.DownloadDirectory, value);
        }

        public string ClientSecret
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.ClientSecret);
            set => _configHelper.SetSetting(SettingNames.ClientSecret, value);
        }

        public string ClientOAuth2Id
        {
            get => _configHelper.GetSettingOrPlaceholder(SettingNames.ClientOAuth2Id);
            set => _configHelper.SetSetting(SettingNames.ClientOAuth2Id, value);
        }
    }
}