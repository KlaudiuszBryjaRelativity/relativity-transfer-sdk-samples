using System.Threading;
using System.Threading.Tasks;

namespace Relativity.Transfer.SDK.Sample.Samples;

internal interface ISample
{
	Task ExecuteAsync(Configuration.Configuration configuration, CancellationToken token);
}