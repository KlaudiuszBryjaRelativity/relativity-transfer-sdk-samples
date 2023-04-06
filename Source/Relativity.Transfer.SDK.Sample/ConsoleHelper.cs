namespace Relativity.Transfer.SDK.Sample
{
	using Relativity.Transfer.SDK.Sample.Samples;
	using Relativity.Transfer.SDK.Sample.Authentication.Credentials;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Relativity.Transfer.SDK.Interfaces.Paths;
	using Relativity.Transfer.SDK.Sample.Authentication;
	using System.Text;

	public static class SettingNames
	{
		public const string ClientName = "ClientName";
		public const string RelativityOneInstanceUrl = "RelativityOneInstanceUrl";
		public const string RelativityOneFileshareRoot = "RelativityOneFileshareRoot";
		public const string FileshareRelativeDestinationPath = "FileshareRelativeDestinationPath";
		public const string ClientSecret = "ClientSecret";
		public const string ClientLogin = "ClientLogin";
		public const string ClientPassword = "ClientPassword";
		public const string ClientOAuth2Id = "ClientOAuth2Id";
	}

	public class ConsoleHelper
	{
		// Main menu keys 
		private const char ConfigureSettingsKey = 'C';
		private const char SetupBasicAuthentication = 'B';
		private const char SetupSecretAuthentication = 'S';
		private const char ExitKey = 'X';

		// other available keys 
		private readonly char[] Keys = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F'};

		private readonly ConfigHelper _configHelper;
		private readonly Dictionary<char, SampleBase> _samples;
		private readonly Dictionary<char, string> _settings;

		private enum MenuState
		{
			Main = 0,
			SetupSettings = 1,
		}

		private MenuState _menuState = MenuState.Main;

		public ConsoleHelper(ConfigHelper configHelper)
		{
			_configHelper = configHelper;
			_samples = new Dictionary<char, SampleBase>();
			_settings = new Dictionary<char, string>();
			int index = 0;
			foreach( var setting in _configHelper.AllSettingsKeys)
			{
				_settings.Add(Keys[index], setting);
				++index;
			}
		}

		public void RegisterSample(char key, SampleBase sample) 
		{
			_samples.Add(key, sample);
		}

		public BasicCredentials BasicCredentials { get; private set; }
		public SecretCredentials SecretCredentials { get; private set; }

		public bool BasicCredentialsValid {  get { return BasicCredentials != null; } }
		public bool SecretCredentialsValid {  get { return SecretCredentials != null;} }

		public async Task<bool> InitStartupSettingsAsync()
		{
			try
			{
				Console.SetWindowSize(120, 30);
			}
			catch(Exception)
			{
			}
			Console.ResetColor();
			Console.Clear();
			PrintHeader();
			Console.WriteLine($" Thanks for using Relativity Transfer SDK v.{GetSDKVersion()}!");
			PrintHeader();
			Console.WriteLine($"  - Now we will ask you to provide neccessary settings to run samples.");
			Console.WriteLine($"  - Some of them, like credentials or instance URL will be validated by reaching authentication endpoint.");
			Console.WriteLine($"  - Most of them will be stored in config file, so you no need to provide them again after restart.");
			Console.WriteLine($"  - Rebuild the application will reset the filled config file.");
			Console.WriteLine($"  - Keep in mind that you can fill App.config file from the repository to have settings stored pernamently.");
			Console.WriteLine($"  - Storing pernamently user secret is not recomended, as it has short expiration time (usually 8 hours).");
			Console.WriteLine();

			string clientName = GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceUrl = GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string relativityFileshareRoot = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
			string clientOauthId = GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientLogin = GetOrEnterSetting(SettingNames.ClientLogin);
			string clientPassword = GetOrEnterSetting(SettingNames.ClientPassword);
			string clientSecret;

			try
			{
				Console.Write("  Refreshing client secret...");
				string base64BasicCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientLogin + ":" + clientPassword));
				OAuthClientManager oauthClientManager = new OAuthClientManager(relativityInstanceUrl, clientOauthId, base64BasicCredentials);
				clientSecret = await oauthClientManager.RetrieveClientSecretAsync();
				Console.WriteLine($" Success!");
				Console.WriteLine($"  {"Client Secret", -40}: {clientSecret}");
			}
			catch ( Exception ex) 
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"  Error while refreshing use secret: {ex.Message}");
				Console.WriteLine("  Ensure provided settings are correct.");
				return false;
			}

			_configHelper.SetSetting(SettingNames.ClientName, clientName);
			_configHelper.SetSetting(SettingNames.RelativityOneInstanceUrl, relativityInstanceUrl);
			_configHelper.SetSetting(SettingNames.RelativityOneFileshareRoot, relativityFileshareRoot);
			_configHelper.SetSetting(SettingNames.ClientLogin, clientLogin);
			_configHelper.SetSetting(SettingNames.ClientOAuth2Id, clientOauthId);
			_configHelper.SetSetting(SettingNames.ClientSecret, clientSecret);

			Console.WriteLine($"  Settings updated successfully. Press any key to continue... ");
			Console.ReadKey();
			Console.Write("\b \b");
			return true;
		}

		public async Task RunMenuAsync()
		{
			bool printMenu = true;
			while(true)
			{
				if( printMenu)
				{
					PrintMenu();
					printMenu = false;
				}
				char key = char.ToUpper(Console.ReadKey().KeyChar);
				Console.Write("\b \b");

				switch (_menuState)
				{
					case MenuState.Main:
						if( key == ExitKey)
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
			string settingValue = _configHelper.GetSetting(settingName);
			if( string.IsNullOrEmpty(settingValue) )
			{
				string enterValuePrefix = $"Enter new {settingName}";
				Console.Write($"  {enterValuePrefix, -40}: ");
				if( settingName == SettingNames.ClientPassword)
				{
					settingValue = GetPassword();
				} else
				{
					settingValue = Console.ReadLine();
				}
			} 
			else if(printValueIfAlreadySet)
			{
				string currentValuePrefix = $"Current {settingName}";
				Console.WriteLine($"  {currentValuePrefix, -40}: {settingValue}");
			}
			return settingValue;
		}

		private string GetPassword()
		{
			var pwd = string.Empty;
			while (true)
			{
				ConsoleKeyInfo i = Console.ReadKey(true);
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				}
				if (i.Key == ConsoleKey.Backspace)
				{
					if (pwd.Length > 0)
					{
						pwd.Remove(pwd.Length - 1);
						Console.Write("\b \b");
					}
				}
				else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
				{
					pwd += i.KeyChar;
					Console.Write("*");
				}
			}
			Console.WriteLine();

			return pwd;
		}


		private void PrintMenu()
		{
			Console.ResetColor();
			Console.Clear();
			PrintHeader();
			Console.WriteLine($" Thanks for using Relativity Transfer SDK v.{GetSDKVersion()}!");
			PrintHeader();
			//Console.WriteLine(" Authentication: ");
			//Console.WriteLine($"    {"Basic Credentials",-20}: {(BasicCredentials == null ? "Not set - press \"B\" to set" : $"User: {_configHelper.GetSetting(SettingNames.ClientLogin)}")}");
			//Console.WriteLine($"    {"Secret Credentials",-20}: {(SecretCredentials == null ? "Not set - press \"S\" to set" : $"User id: {_configHelper.GetSetting(SettingNames.ClientOAuth2Id)}")}");
			//PrintHeader();
			Console.WriteLine(" Stored settings: ");
			foreach( string key in _configHelper.AllSettingsKeys)
			{
				Console.WriteLine($"    {key.PadRight(35)}:  \"{_configHelper.GetSettingOrPlaceholder(key)}\"");
			}
			PrintHeader();
			switch(_menuState)
			{
				case MenuState.Main: PrintMainMenu(); break;
				case MenuState.SetupSettings: PrintSetupSettingsMenu(); break;
			}			
		}

		private void PrintMainMenu()
		{
			_menuState = MenuState.Main;
			Console.WriteLine(" Actions: ");
			Console.WriteLine($"  - [{SetupBasicAuthentication}] - Setup User Basic credentials");
			Console.WriteLine($"  - [{SetupSecretAuthentication}] - Setup User Secret credentials");
			Console.WriteLine($"  - [{ConfigureSettingsKey}] - Configure settings");
			Console.WriteLine($"  - [{ExitKey}] - Exit");
			PrintHeader();
			Console.WriteLine(" Samples: ");
			foreach ( var kv in _samples)
			{
				Console.WriteLine($"  - [{kv.Key}] - Start Sample {kv.Value.Descritpion}");
			}
			PrintHeader();
		}

		private void PrintSetupSettingsMenu()
		{
			Console.WriteLine(" Settings: ");
			foreach ( var kv in _settings)
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
			else if ( key == SetupBasicAuthentication)
			{
				EnterBasicCredentials();
				return true;
			}
			else if( key == SetupSecretAuthentication)
			{
				EnterSecretCredentials();
				return true;
			}
			else
			{
				foreach (var kv in _samples)
				{
					if (kv.Key == key)
					{
						Console.WriteLine($"   Executing sample \"{kv.Value.Descritpion}\"");
						PrintHeader();
						// here -> Executing the sample
						try
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							await kv.Value.ExecuteAsync().ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine($"Exception while executing sample {kv.Value.GetType()}: {ex.Message}");
							Console.WriteLine(ex.ToString());
							Console.ResetColor();
						}
						PrintHeader();
						Console.WriteLine($" The Sample {kv.Key}:  \"{kv.Value.Descritpion}\" has finished. Press any key to continue... ");
						Console.ReadKey();
						return true;
					}
				}
			}

			return false;
		}


		private void EnterBasicCredentials()
		{
			BasicCredentials = null;
			Console.Write("Enter User Relativity login: ");
			string login = Console.ReadLine();
			Console.Write("Enter User password: ");
			string password = GetPassword();
			Console.Write("Enter User Authentication ID: ");
			string userID = Console.ReadLine();
			if( string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userID))
			{
				return;
			}
			_configHelper.SetSetting(SettingNames.ClientLogin, login);
			_configHelper.SetSetting(SettingNames.ClientOAuth2Id, userID);

			BasicCredentials = new BasicCredentials(login, password, userID);
		}

		private void EnterSecretCredentials()
		{
			SecretCredentials = null;
			Console.Write("Enter User Authentication ID: ");
			string userID = Console.ReadLine();
			Console.Write("Enter User Secret: ");
			string userSecret = Console.ReadLine();
			if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userSecret))
			{
				return;
			}

			_configHelper.SetSetting(SettingNames.ClientOAuth2Id, userID);
			_configHelper.SetSetting(SettingNames.ClientSecret, userSecret);

			SecretCredentials = new SecretCredentials(userID, userSecret);
		}

		private bool ProcessSetupSettingsAction(char key)
		{
			if (key == ExitKey) 
			{
				_menuState = MenuState.Main;
				return true;
			}
			foreach ( var kv in _settings)
			{
				if( kv.Key == key)
				{
					SetupSetting(kv.Value);
				}
			}
			return true;
		}

		private void SetupSetting(string settingKey)
		{
			Console.Write($"   Enter new value for {settingKey}: ");
			string line = Console.ReadLine();
			_configHelper.SetSetting(settingKey, line);
		}

		private Version GetSDKVersion()
		{
			return typeof(TransferClientBuilder).Assembly.GetName().Version;
		}

		public static void WriteSection(params string[] messages)
		{
			PrintHeader();

			foreach (string msg in messages)
			{
				Console.WriteLine(msg);
			}

			Console.WriteLine();
		}

		public static void WriteLine(string message, ConsoleColor color = default)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ResetColor();
		}
		
		public DirectoryPath GetDestinationDirectoryPath(string finalFolder)
		{
			string fileshareRootPath = GetOrEnterSetting(SettingNames.RelativityOneFileshareRoot);
			string fileshareDestinatinFolder = GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
			return new DirectoryPath(Path.Combine(fileshareRootPath, fileshareDestinatinFolder, finalFolder));
		}

		public DirectoryPath GetSourceDirectoryPath()
		{
			Console.WriteLine("Provide path to the directory you want to upload to RelativityOne:");

			while (true)
			{
				string path = Console.ReadLine();
				if (string.IsNullOrEmpty(path))
				{
					Console.WriteLine("Provided path is empty.");
					continue;
				}

				if (!Directory.Exists(path))
				{
					Console.WriteLine($"{path} does not exist.");
					continue;
				}

				Console.WriteLine();
				return new DirectoryPath(path);
			}
		}

		public FilePath GetSourceFilePath()
		{
			Console.WriteLine("Provide path to the file you want to upload to RelativityOne:");

			while (true)
			{
				string path = Console.ReadLine();
				if (string.IsNullOrEmpty(path))
				{
					Console.WriteLine("Provided path is empty.");
					continue;
				}

				if (!File.Exists(path))
				{
					Console.WriteLine($"{path} does not exist.");
					continue;
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
