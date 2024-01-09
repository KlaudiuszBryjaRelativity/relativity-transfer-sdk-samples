using System;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal sealed class MainMenu : IMainMenu
{
	private readonly ISampleRunner _sampleRunner;
	private readonly IConsoleLogger _consoleLogger;

	public MainMenu(ISampleRunner sampleRunner, IConsoleLogger consoleLogger)
	{
		_sampleRunner = sampleRunner;
		_consoleLogger = consoleLogger;
	}

	public async Task ShowAsync(CancellationToken token, Func<Task> postScreenAction = null)
	{
		while (!token.IsCancellationRequested)
		{
			var selectedAttribute = _consoleLogger.PrintMainMenu();
			if (selectedAttribute == null) break;

			// Run sample
			try
			{
				await _sampleRunner.ExecuteAsync(selectedAttribute, token);
			}
			catch (Exception ex)
			{
				_consoleLogger.PrintError(ex);
			}
		}

		if (postScreenAction != null) await postScreenAction.Invoke();
		Environment.Exit(0);
	}
}