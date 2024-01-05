using System;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal sealed class MainMenu(ISampleRunner sampleRunner, IConsoleLogger consoleLogger) : IMainMenu
{
	public async Task ShowAsync(CancellationToken token, Func<Task> postScreenAction = null)
	{
		while (!token.IsCancellationRequested)
		{
			var selectedAttribute = consoleLogger.PrintMainMenu();
			if (selectedAttribute == null) break;

			// Run sample
			try
			{
				await sampleRunner.ExecuteAsync(selectedAttribute, token);
			}
			catch (Exception ex)
			{
				consoleLogger.PrintError(ex);
			}
		}

		if (postScreenAction != null) await postScreenAction.Invoke();
		Environment.Exit(0);
	}
}