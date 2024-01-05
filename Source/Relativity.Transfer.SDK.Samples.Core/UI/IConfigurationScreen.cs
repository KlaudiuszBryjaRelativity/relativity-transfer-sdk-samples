using Relativity.Transfer.SDK.Samples.Core.Attributes;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal interface IConfigurationScreen
{
	Core.Configuration.Configuration UpdateConfiguration(Core.Configuration.Configuration configuration,
		SampleAttribute sampleAttribute);
}