namespace Relativity.Transfer.SDK.Sample
{
	using System.Threading.Tasks;
	using Helpers;
	using Samples;

	internal class Program
	{
		private static async Task Main()
		{
			ConfigHelper configHelper = new ConfigHelper();
			ConsoleHelper consoleHelper = new ConsoleHelper(configHelper);

			if( !await consoleHelper.InitStartupSettingsAsync())
			{
				return;
			}

			consoleHelper.RegisterSample('1', new Sample1_BearerTokenAuthentication(consoleHelper));
			consoleHelper.RegisterSample('2', new Sample2_BasicCredentialsAndBearerAuthentication(consoleHelper));
			consoleHelper.RegisterSample('3', new Sample3_SettingUpProgressHandlerAndPrintingSummary(consoleHelper));
			consoleHelper.RegisterSample('4', new Sample4_UploadSingleFile(consoleHelper));
			consoleHelper.RegisterSample('5', new Sample5_UploadDirectory(consoleHelper));
			consoleHelper.RegisterSample('6', new Sample6_UploadDirectoryWithCustomizedRetryPolicy(consoleHelper));
			consoleHelper.RegisterSample('7', new Sample7_UploadDirectoryWithExclusionPolicy(consoleHelper));
			consoleHelper.RegisterSample('8', new Sample8_UploadToFilesharePathBasedOnWorkspaceId(consoleHelper));

			await consoleHelper.RunMenuAsync();
		}
	}
}
