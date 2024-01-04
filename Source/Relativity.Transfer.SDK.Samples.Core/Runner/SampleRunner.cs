using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.UI;

namespace Relativity.Transfer.SDK.Samples.Core.Runner;

internal sealed class SampleRunner(
	IEnumerable<ISample> samples,
	Configuration.Configuration configuration,
	IConfigurationScreen configurationScreen)
	: ISampleRunner
{
	public Task ExecuteAsync(SampleAttribute sampleAttribute, CancellationToken token)
	{
		var sample = samples.FirstOrDefault(x => x?.GetType() == sampleAttribute.SampleType);
		if (sample == null) throw new Exception("Something went wrong, No sample could be found.");

		var updatedConfiguration = configurationScreen.UpdateConfiguration(configuration, sampleAttribute);

		return sample.ExecuteAsync(updatedConfiguration, token);
	}
}