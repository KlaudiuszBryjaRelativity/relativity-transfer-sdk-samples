using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.UI;

namespace Relativity.Transfer.SDK.Samples.Core.Services;

internal class MainMenuHostedService(IMainMenu mainMenu, IConsoleLogger logger) : IHostedService
{
	public Task StartAsync(CancellationToken token)
	{
		return mainMenu.ShowAsync(token, () => StopAsync(token));
	}

	public Task StopAsync(CancellationToken token)
	{
		return logger.PrintExitMessageAsync();
	}
}