using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Relativity.Transfer.SDK.Sample.Helpers;
using Relativity.Transfer.SDK.Sample.UI;

namespace Relativity.Transfer.SDK.Sample.Services;

internal class MainMenuHostedService : IHostedService
{
	private readonly IConsoleLogger _logger;
	private readonly IMainMenu _mainMenu;

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