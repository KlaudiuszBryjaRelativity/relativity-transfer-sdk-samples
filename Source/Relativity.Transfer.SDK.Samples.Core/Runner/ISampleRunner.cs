using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Samples.Core.Attributes;

namespace Relativity.Transfer.SDK.Samples.Core.Runner;

internal interface ISampleRunner
{
	Task ExecuteAsync(SampleAttribute sampleAttribute, CancellationToken token);
}