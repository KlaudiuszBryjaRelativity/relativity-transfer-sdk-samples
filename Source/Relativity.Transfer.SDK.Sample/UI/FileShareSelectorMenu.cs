using System.Linq;
using System.Threading;
using Relativity.Transfer.SDK.Sample.DTOs;
using Relativity.Transfer.SDK.Sample.Helpers;

namespace Relativity.Transfer.SDK.Sample.UI;

internal sealed class FileShareSelectorMenu : IFileShareSelectorMenu
{
	private readonly IConsoleLogger _consoleLogger;

	public FileShareSelectorMenu(IConsoleLogger consoleLogger)
	{
		_consoleLogger = consoleLogger;
	}

	public FileShareInfo SelectFileShare(FileShareInfo[] fileShareInfos, CancellationToken token)
	{
		if (!fileShareInfos.Any())
		{
			_consoleLogger.Info("[red]There are no file shares for provided workspace Id.[/]");

			return null;
		}

		if (fileShareInfos.Length == 1)
		{
			var uncPath = fileShareInfos[0].UncPath;
			_consoleLogger.Info(
				$"[yellow]There is only one file share for provided workspace Id. Its UNCPath is[/] [green]{uncPath}[/]");

			return fileShareInfos[0];
		}

		return _consoleLogger.PrintFileShareInfosMenu(fileShareInfos);
	}
}