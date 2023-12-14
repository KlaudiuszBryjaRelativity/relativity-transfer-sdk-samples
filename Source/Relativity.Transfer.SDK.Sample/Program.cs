using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Relativity.Transfer.SDK.Sample.Authentication;
using Relativity.Transfer.SDK.Sample.Helpers;
using Relativity.Transfer.SDK.Sample.ProgressHandler;
using Relativity.Transfer.SDK.Sample.Runner;
using Relativity.Transfer.SDK.Sample.Samples;
using Relativity.Transfer.SDK.Sample.Services;
using Relativity.Transfer.SDK.Sample.UI;
using SampleConfigurationProvider = Relativity.Transfer.SDK.Sample.Configuration.ConfigurationProvider;

namespace Relativity.Transfer.SDK.Sample;

internal class Program
{
	private static async Task Main()
	{
		await Host.CreateDefaultBuilder()
			.ConfigureServices((_, services) =>
			{
				// Register UI and common services
				services.AddHostedService<MainMenuHostedService>();
				services.AddSingleton<IConsoleLogger, ConsoleLogger>();
				services.AddSingleton<IMainMenu, MainMenu>();
				services.AddSingleton<ISampleRunner, SampleRunner>();
				services.AddSingleton<IConfigurationScreen, ConfigurationScreen>();
				services.AddSingleton<IPathExtension, PathExtension>();
				services.AddSingleton<IBearerTokenRetriever, BearerTokenRetriever>();
				services
					.AddSingleton<IRelativityAuthenticationProviderFactory, RelativityAuthenticationProviderFactory>();
				services.AddSingleton<IProgressHandlerFactory, ProgressHandlerFactory>();
				services.AddSingleton<IFileShareSelectorMenu, FileShareSelectorMenu>();

				// Register configuration
				services.AddSingleton(_ => SampleConfigurationProvider.GetConfiguration());

				// Register samples
				services.AddTransient<ISample, BearerTokenAuthentication>();
				services.AddTransient<ISample, SettingUpProgressHandlerAndPrintingSummary>();
				services.AddTransient<ISample, UploadFile>();
				services.AddTransient<ISample, UploadDirectory>();
				services.AddTransient<ISample, UploadDirectoryWithCustomizedRetryPolicy>();
				services.AddTransient<ISample, UploadDirectoryWithExclusionPolicy>();
				services.AddTransient<ISample, UploadToFileSharePathBasedOnWorkspaceId>();
				services.AddTransient<ISample, DownloadFile>();
				services.AddTransient<ISample, DownloadDirectory>();
			})
			.ConfigureLogging((_, cfg) => { cfg.SetMinimumLevel(LogLevel.Warning); })
			.RunConsoleAsync();
	}
}