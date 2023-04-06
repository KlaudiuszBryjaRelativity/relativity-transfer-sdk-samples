namespace Relativity.Transfer.SDK.Sample
{
	using Relativity.Transfer.SDK.Sample.Samples;
	using System.Threading.Tasks;

	internal class Program
	{
		// pomysly: 
		 // [done]  poprosic na starcie uzytkonika o podanie wszystkich danych + walidacja ?? + refresh secretu

		// dodac w readme: potencjalne bledy / trouble shooting in case of bad path / wrong secret etc... 
		// pamietac ze secret jest valid 480 minut czyli 8h -> nalezy go regularnie odswiezac 
		// pernament save config file -> z tym bylo ciezko, moze poprostu opisac w readme zeby podmienic 

		private static async Task Main()
		{
			ConfigHelper configHelper = new ConfigHelper();
			ConsoleHelper consoleHelper = new ConsoleHelper(configHelper);

			// ensure all setting are set 
			if( !await consoleHelper.InitStartupSettingsAsync())
			{
				return;
			}

			consoleHelper.RegisterSample('1', new Sample1_ClientSecretAuthentication(consoleHelper));
			consoleHelper.RegisterSample('2', new Sample2_BasicCredentialsAuthentication(consoleHelper));
			consoleHelper.RegisterSample('3', new Sample3_SettingUpProgressHandler(consoleHelper));
			consoleHelper.RegisterSample('4', new Sample4_UploadSingleFile(consoleHelper));
			consoleHelper.RegisterSample('5', new Sample5_UploadDirectory(consoleHelper));
			consoleHelper.RegisterSample('6', new Sample6_UploadDirectoryWithCustomizedRetryPolicy(consoleHelper));
			consoleHelper.RegisterSample('7', new Sample7_UploadDirectoryWithExclusionPolicy(consoleHelper));

			await consoleHelper.RunMenuAsync();
		}
	}
}
