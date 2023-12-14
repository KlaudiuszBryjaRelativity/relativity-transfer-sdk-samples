using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Sample.Attributes;
using Relativity.Transfer.SDK.Sample.Samples;
using Relativity.Transfer.SDK.Sample.UI;

namespace Relativity.Transfer.SDK.Sample.Runner;

internal sealed class SampleRunner : ISampleRunner
{
	private readonly Configuration.Configuration _configuration;
	private readonly IConfigurationScreen _configurationScreen;
	private readonly IEnumerable<ISample> _samples;

	public SampleRunner(IEnumerable<ISample> samples, Configuration.Configuration configuration,
		IConfigurationScreen configurationScreen)
	{
		_samples = samples;
		_configuration = configuration;
		_configurationScreen = configurationScreen;
	}

	public Task ExecuteAsync(SampleAttribute sampleAttribute, CancellationToken token)
	{
		var sample = _samples.FirstOrDefault(x => x?.GetType() == sampleAttribute.SampleType);
		if (sample == null) throw new Exception("Something went wrong, No sample could be found.");

		var updatedConfiguration = _configurationScreen.UpdateConfiguration(_configuration, sampleAttribute);

		return sample.ExecuteAsync(updatedConfiguration, token);
	}
}