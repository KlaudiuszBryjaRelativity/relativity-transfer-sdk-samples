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
			ConfigProvider configProvider = new ConfigProvider(configHelper);
			ConsoleHelper consoleHelper = new ConsoleHelper(configHelper, configProvider);
			DownloadConsoleHelper downloadConsoleHelper = new DownloadConsoleHelper(consoleHelper, configProvider);

			if( !consoleHelper.InitStartupSettings())
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
			consoleHelper.RegisterSample('8', new Sample8_DownloadSingleFile(downloadConsoleHelper));
			consoleHelper.RegisterSample('9', new Sample9_DownloadDirectory(downloadConsoleHelper));

			await consoleHelper.RunMenuAsync();
		}
	}
}
