using Relativity.Transfer.SDK.Interfaces.ProgressReporting;

namespace Relativity.Transfer.SDK.Sample.ProgressHandler;

internal interface IProgressHandlerFactory
{
	ITransferProgressHandler Create();
}