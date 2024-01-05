using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Interfaces.Authentication;

namespace Relativity.Transfer.SDK.Samples.Core.Authentication;

internal sealed class RelativityAuthenticationProviderFactory(
	IConsoleLogger consoleLogger,
	IBearerTokenRetriever bearerTokenRetriever)
	: IRelativityAuthenticationProviderFactory
{
	public IRelativityAuthenticationProvider Create(CommonConfiguration common)
	{
		return new RelativityAuthenticationProvider(common, consoleLogger, bearerTokenRetriever);
	}
}