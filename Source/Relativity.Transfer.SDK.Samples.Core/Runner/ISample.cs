using System.Threading;
using System.Threading.Tasks;

namespace Relativity.Transfer.SDK.Samples.Core.Runner;

internal interface ISample
{
	Task ExecuteAsync(Configuration.Configuration configuration, CancellationToken token);
}