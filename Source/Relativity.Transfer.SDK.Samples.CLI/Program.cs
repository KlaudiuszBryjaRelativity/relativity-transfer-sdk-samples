using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Relativity.Transfer.SDK.Samples.Core.Authentication;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.ProgressHandler;
using Relativity.Transfer.SDK.Samples.Core.Runner;
using Relativity.Transfer.SDK.Samples.Core.Services;
using Relativity.Transfer.SDK.Samples.Core.UI;
using Relativity.Transfer.SDK.Samples.Repository;

namespace Relativity.Transfer.SDK.Samples.CLI;

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
				services.AddSingleton(_ => ConfigurationProvider.GetConfiguration());

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