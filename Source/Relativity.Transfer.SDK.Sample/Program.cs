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
			consoleHelper.RegisterSample('2', new Sample2_SettingUpProgressHandlerAndPrintingSummary(consoleHelper));
			consoleHelper.RegisterSample('3', new Sample3_UploadSingleFile(consoleHelper));
			consoleHelper.RegisterSample('4', new Sample4_UploadDirectory(consoleHelper));
			consoleHelper.RegisterSample('5', new Sample5_UploadDirectoryWithCustomizedRetryPolicy(consoleHelper));
			consoleHelper.RegisterSample('6', new Sample6_UploadDirectoryWithExclusionPolicy(consoleHelper));
			consoleHelper.RegisterSample('7', new Sample7_UploadToFilesharePathBasedOnWorkspaceId(consoleHelper));

			await consoleHelper.RunMenuAsync();
		}
	}
}
