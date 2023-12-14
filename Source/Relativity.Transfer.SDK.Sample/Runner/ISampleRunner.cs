using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Sample.Attributes;

namespace Relativity.Transfer.SDK.Sample.Runner;

internal interface ISampleRunner
{
	Task ExecuteAsync(SampleAttribute sampleAttribute, CancellationToken token);
}