using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Relativity.Transfer.SDK.Sample
{
	public class ConfigHelper
	{
		private const string BaseConfigNameFileName = "App.config";
		private const string NotSetConfigString = "<not set>";

		private readonly Configuration _appConfig;

		public IEnumerable<string> AllSettingsKeys => _appConfig.AppSettings.Settings.AllKeys;

		public ConfigHelper() 
		{
			_appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}

		public string GetSetting(string settingKey)
		{
			if (!_appConfig.AppSettings.Settings.AllKeys.Contains(settingKey))
			{
				return string.Empty;
			}
			return _appConfig.AppSettings.Settings[settingKey].Value;
		}

		public string GetSettingOrPlaceholder(string settingKey)
		{
			if (!_appConfig.AppSettings.Settings.AllKeys.Contains(settingKey))
			{
				return NotSetConfigString;
			}
			string value = _appConfig.AppSettings.Settings[settingKey].Value;
			if(string.IsNullOrEmpty(value))
			{
				return NotSetConfigString;
			}
			return value;
		}

		public void SetSetting(string settingKey, string value)
		{
			_appConfig.AppSettings.Settings[settingKey].Value = value;
			_appConfig.Save(ConfigurationSaveMode.Full, true);
			ConfigurationManager.RefreshSection("appSettings");
		}

		public void ReplaceBaseConfig()
		{
			string baseConfigPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, BaseConfigNameFileName);
			throw new NotImplementedException();
		}

		public void CleanupBaseConfig()
		{
			string baseConfigPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, BaseConfigNameFileName);
			throw new NotImplementedException();
		}

	}
}
