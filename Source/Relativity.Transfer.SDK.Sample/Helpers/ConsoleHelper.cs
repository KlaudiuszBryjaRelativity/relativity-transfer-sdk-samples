namespace Relativity.Transfer.SDK.Sample.Helpers
{
	using System;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Runtime.InteropServices;
	using Samples;
	using Interfaces.Paths;

	internal class ConsoleHelper : IConsoleHelper
	{
		// Main menu keys 
		private const char ConfigureSettingsKey = 'C';
		private const char DeleteTemporaryData = 'D';
		private const char ExitKey = 'X';

		// other available keys 
		private readonly char[] Keys = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F' };

		private readonly ConfigHelper _configHelper;
		private readonly IConfigProvider _configProvider;
		private readonly Dictionary<char, SampleBase> _samples;
		private readonly Dictionary<char, string> _settings;
		
		private enum MenuState
		{
			Main = 0,
			SetupSettings = 1,
		}

		private MenuState _menuState = MenuState.Main;
		
		public ConsoleHelper(ConfigHelper configHelper, IConfigProvider configProvider)
		{
			_configHelper = configHelper;
			_configProvider = configProvider;
			_samples = new Dictionary<char, SampleBase>();
			_settings = new Dictionary<char, string>();
			var index = 0;
			foreach (var setting in _configHelper.AllSettingsKeys)
			{
				_settings.Add(Keys[index], setting);
				++index;
			}
		}

		public void RegisterSample(char key, SampleBase sample)
		{
			_samples.Add(key, sample);
		}

		public bool InitStartupSettings()
		{
			Console.ResetColor();
			Console.Clear();
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					Console.SetWindowSize(120, 30);
				}
			}
			catch (Exception)
			{
			}
			PrintHeader();
			Console.WriteLine($" Thanks for using Relativity Transfer SDK v.{GetSDKVersion()}!");
			PrintHeader();
			Console.WriteLine("  - Now we will ask you to provide necessary settings to run samples.");
			Console.WriteLine("  - Some of them, like credentials or instance URL will be validated by reaching authentication endpoint.");
			Console.WriteLine("  - You can leave Default Source Paths empty for now. You will be asked for them when needed.");
			Console.WriteLine("  - All settings will be stored in config file (excluding the password), to streamline the experience with the samples.");
			Console.WriteLine("  - Application recompilation will flush saved settings.");
			Console.WriteLine("  - You can set all configuration values directly in App.config to store them permanently.");
			Console.WriteLine("  - Storing permanently user secret is not recommended, as it has short expiration time (8 hours by default).");
			Console.WriteLine();

			var clientName = GetOrEnterSetting(SettingNames.ClientName);
			var relativityInstanceUrl = GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			var relativityFileshareRoot = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
			var defaultSourceFilePath = GetOrEnterSetting(SettingNames.DefaultSourceFilePath);
			var defaultSourceDirectoryPath = GetOrEnterSetting(SettingNames.DefaultSourceDirectoryPath);
			var clientOauthId = GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			var clientSecret = GetOrEnterSetting(SettingNames.ClientSecret);
			var downloadCatalog = GetOrEnterSetting(SettingNames.DownloadCatalog);

			_configProvider.ClientName = clientName;
			_configProvider.RelativityOneInstanceUrl = relativityInstanceUrl;
			_configProvider.RelativityOneFileshareRoot = relativityFileshareRoot;
			_configProvider.DefaultSourceFilePath = defaultSourceFilePath;
			_configProvider.DefaultSourceDirectoryPath = defaultSourceDirectoryPath;
			_configProvider.ClientOAuth2Id = clientOauthId;
			_configProvider.ClientSecret = clientSecret;
			_configProvider.DownloadCatalog = downloadCatalog;

			Console.WriteLine($"  Settings updated successfully. Press any key to continue... ");
			Console.ReadKey();
			Console.Write("\b \b");
			return true;
		}

		public async Task RunMenuAsync()
		{
			var printMenu = true;
			while (true)
			{
				if (printMenu)
				{
					PrintMenu();
					printMenu = false;
				}
				var key = char.ToUpper(Console.ReadKey().KeyChar);
				Console.Write("\b \b");

				switch (_menuState)
				{
					case MenuState.Main:
						if (key == ExitKey)
						{
							return;
						}
						printMenu = await ProcessMainMenuAction(key);
						break;
					case MenuState.SetupSettings:
						printMenu = ProcessSetupSettingsAction(key);
						break;
					default:
						throw new InvalidOperationException();
				}
			}
		}

		public string GetOrEnterSetting(string settingName, bool printValueIfAlreadySet = true)
		{
			var settingValue = _configHelper.GetSetting(settingName);
			if (string.IsNullOrEmpty(settingValue))
			{
				var enterValuePrefix = $"Enter {settingName}";
				Console.Write($"  {enterValuePrefix,-40}: ");
				settingValue = Console.ReadLine();
			}
			else if (printValueIfAlreadySet)
			{
				var currentValuePrefix = $"Current {settingName}";
				Console.WriteLine($"  {currentValuePrefix,-40}: {settingValue}");
			}
			return settingValue;
		}

		public string EnterUntilValid(string askingtext, string validationPattern)
		{
			var regex = new Regex(validationPattern);
			while (true)
			{
				Console.Write($"  {askingtext,-40}: ");
				var line = Console.ReadLine();

				if (regex.Match(line).Success)
				{
					return line;
				}
				else
				{
					var validationFailStr = "Provided value is not valid, please try again";
					Console.WriteLine($"  {validationFailStr,-40}");
				}
			}
		}

		private void PrintMenu()
		{
			Console.ResetColor();
			Console.Clear();
			PrintHeader();
			Console.WriteLine($" Thanks for using Relativity Transfer SDK v.{GetSDKVersion()}!");
			PrintHeader();
			Console.WriteLine(" Stored settings: ");
			foreach (string key in _configHelper.AllSettingsKeys)
			{
				Console.WriteLine($"    {key,-35}:  \"{_configHelper.GetSettingOrPlaceholder(key)}\"");
			}
			PrintHeader();
			switch (_menuState)
			{
				case MenuState.Main: PrintMainMenu(); break;
				case MenuState.SetupSettings: PrintSetupSettingsMenu(); break;
			}
		}

		private void PrintMainMenu()
		{
			_menuState = MenuState.Main;
			Console.WriteLine(" Actions: ");
			Console.WriteLine($"  - [{ConfigureSettingsKey}] - Configure settings");
			Console.WriteLine($"  - [{DeleteTemporaryData}] - Cleanup temporary transfer data");
			Console.WriteLine($"  - [{ExitKey}] - Exit");
			PrintHeader();
			Console.WriteLine(" Samples: ");
			foreach (var kv in _samples)
			{
				Console.WriteLine($"  - [{kv.Key}] - Start Sample {kv.Value.Description}");
			}
			PrintHeader();
		}

		private void PrintSetupSettingsMenu()
		{
			Console.WriteLine(" Settings: ");
			foreach (var kv in _settings)
			{
				Console.WriteLine($"  - [{kv.Key}] - change \"{kv.Value}\"");
			}
			Console.WriteLine($"  - [{ExitKey}] - Exit");
		}

		private async Task<bool> ProcessMainMenuAction(char key)
		{
			if (key == ConfigureSettingsKey)
			{
				_menuState = MenuState.SetupSettings;
				return true;
			}
			else if(key == DeleteTemporaryData)
			{
				Console.Clear();
				Console.WriteLine();
				Console.WriteLine(" Removing TransferSDK temporary files...");
				SourceFilesHelper.ClearTransferSdkTemporaryData();
				ClearConsoleBuffer();
				Console.WriteLine(" The files are removed. Press any key to continue... ");
				Console.ReadKey();
				return true;
			}
			else
			{
				foreach (var kv in _samples)
				{
					if (kv.Key == key)
					{
						Console.Clear();
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.WriteLine();
						Console.WriteLine($"Executing sample \"{kv.Value.Description}\"");
						Console.WriteLine();
						try
						{
							await kv.Value.ExecuteAsync().ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine($"Exception while executing sample {kv.Value.GetType()}: {ex.Message}");
							Console.WriteLine(ex.ToString());
							Console.ResetColor();
						}
						ClearConsoleBuffer();
						Console.WriteLine();
						Console.WriteLine($" The Sample {kv.Key}:  \"{kv.Value.Description}\" has finished. Press any key to continue... ");
						Console.ReadKey();
						return true;
					}
				}
			}

			return false;
		}

		private void ClearConsoleBuffer()
		{
			while (Console.KeyAvailable)
				Console.ReadKey(true);
		}

		private bool ProcessSetupSettingsAction(char key)
		{
			if (key == ExitKey)
			{
				_menuState = MenuState.Main;
				return true;
			}
			foreach (var kv in _settings)
			{
				if (kv.Key == key)
				{
					SetupSetting(kv.Value);
				}
			}
			return true;
		}

		private void SetupSetting(string settingKey)
		{
			Console.Write($"   Enter new value for {settingKey}: ");
			var line = Console.ReadLine();
			_configHelper.SetSetting(settingKey, line);
		}

		private Version GetSDKVersion()
		{
			return typeof(TransferClientBuilder).Assembly.GetName().Version;
		}

		public DirectoryPath GetDestinationDirectoryPath(string transferJobId)
		{
			var fileshareRootPath = _configProvider.RelativityOneFileshareRoot;
			var fileshareDestinationFolder = _configProvider.FileshareRelativeDestinationPath;
			return new DirectoryPath(Path.Combine(fileshareRootPath, fileshareDestinationFolder, transferJobId));
		}

		public DirectoryPath EnterSourceDirectoryPathOrTakeDefault()
		{
			Console.WriteLine($"  Provide path to the directory you want to {nameof(TransferDirection.Upload)} to RelativityOne:");
			Console.WriteLine($"	 (keep it empty to use default path: \"{_configProvider.DefaultSourceDirectoryPath}\"");

			var overwriteDefaultSetting = false;
			while (true)
			{
				Console.Write("  Directory Path: ");
				var path = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(path))
				{
					path = _configProvider.DefaultSourceDirectoryPath;
					overwriteDefaultSetting = true;
				}

				if (!Directory.Exists(path))
				{
					Console.WriteLine($"  Directory \"{path}\" does not exist.");
					continue;
				}

				if (overwriteDefaultSetting)
				{
					_configProvider.DefaultSourceDirectoryPath = path;
				}

				Console.WriteLine();
				return new DirectoryPath(path);
			}
		}

		public virtual FilePath EnterSourceFilePathOrTakeDefault()
		{
			Console.WriteLine($"  Provide path to the file you want to {nameof(TransferDirection.Upload)} to RelativityOne:");
			Console.WriteLine($"	 (keep it empty to use default path: \"{_configProvider.DefaultSourceFilePath}\"");

			var overwriteDefaultSetting = false;
			while (true)
			{
				Console.Write("  File Path: ");
				var path = Console.ReadLine();
				if (string.IsNullOrWhiteSpace(path))
				{
					path = _configProvider.DefaultSourceFilePath;
					overwriteDefaultSetting = true;

				}

				if (!File.Exists(path))
				{
					Console.WriteLine($"  File \"{path}\" does not exist.");
					continue;
				}

				if (overwriteDefaultSetting)
				{
					_configProvider.DefaultSourceFilePath = path;
				}

				Console.WriteLine();
				return new FilePath(path);
			}
		}

		private static void PrintHeader()
		{
			Console.WriteLine("---------------------------------------------------------");
		}
	}
}
