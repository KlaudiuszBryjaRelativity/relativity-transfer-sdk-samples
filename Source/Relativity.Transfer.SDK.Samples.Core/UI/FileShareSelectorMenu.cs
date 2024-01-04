using System.Linq;
using System.Threading;
using Relativity.Transfer.SDK.Samples.Core.DTOs;
using Relativity.Transfer.SDK.Samples.Core.Helpers;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal sealed class FileShareSelectorMenu(IConsoleLogger consoleLogger) : IFileShareSelectorMenu
{
	public FileShareInfo SelectFileShare(FileShareInfo[] fileShareInfos, CancellationToken token)
	{
		if (!fileShareInfos.Any())
		{
			consoleLogger.Info("[red]There are no file shares for provided workspace Id.[/]");

			return null;
		}

		if (fileShareInfos.Length == 1)
		{
			var uncPath = fileShareInfos[0].UncPath;
			consoleLogger.Info(
				$"[yellow]There is only one file share for provided workspace Id. Its UNCPath is[/] [green]{uncPath}[/]");

			return fileShareInfos[0];
		}

		return consoleLogger.PrintFileShareInfosMenu(fileShareInfos);
	}
}