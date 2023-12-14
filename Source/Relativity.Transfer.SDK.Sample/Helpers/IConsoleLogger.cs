using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Interfaces.ProgressReporting;
using Relativity.Transfer.SDK.Sample.Attributes;
using Relativity.Transfer.SDK.Sample.DTOs;

namespace Relativity.Transfer.SDK.Sample.Helpers;

internal interface IConsoleLogger
{
	Task PrintExitMessageAsync();
	void PrintCreatingTransfer(Guid jobId, PathBase source, PathBase destination, params string[] additionalLines);
	void PrintTransferResult(TransferJobResult result);
	void Info(string msg);
	void PrintError(Exception exception);
	SampleAttribute PrintMainMenu();
	FileShareInfo PrintFileShareInfosMenu(IEnumerable<FileShareInfo> fileShareInfos);
}