using Relativity.Transfer.SDK.Sample.Attributes;

namespace Relativity.Transfer.SDK.Sample.UI;

internal interface IConfigurationScreen
{
	Configuration.Configuration UpdateConfiguration(Configuration.Configuration configuration,
		SampleAttribute sampleAttribute);
}