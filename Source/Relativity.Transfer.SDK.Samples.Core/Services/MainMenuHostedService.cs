using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.UI;

namespace Relativity.Transfer.SDK.Samples.Core.Services;

internal class MainMenuHostedService : IHostedService
{
	private readonly IMainMenu _mainMenu;
	private readonly IConsoleLogger _logger;

	public MainMenuHostedService(IMainMenu mainMenu, IConsoleLogger logger)
	{
		_mainMenu = mainMenu;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken token)
	{
		return _mainMenu.ShowAsync(token, () => StopAsync(token));
	}

	public Task StopAsync(CancellationToken token)
	{
		return _logger.PrintExitMessageAsync();
	}
}