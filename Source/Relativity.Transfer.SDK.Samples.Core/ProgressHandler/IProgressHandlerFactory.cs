using Relativity.Transfer.SDK.Interfaces.ProgressReporting;

namespace Relativity.Transfer.SDK.Samples.Core.ProgressHandler;

internal interface IProgressHandlerFactory
{
	ITransferProgressHandler Create();
}