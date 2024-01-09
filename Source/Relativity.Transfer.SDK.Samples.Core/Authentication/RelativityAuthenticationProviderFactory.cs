using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Interfaces.Authentication;

namespace Relativity.Transfer.SDK.Samples.Core.Authentication;

internal sealed class RelativityAuthenticationProviderFactory : IRelativityAuthenticationProviderFactory
{
	private readonly IConsoleLogger _consoleLogger;
	private readonly IBearerTokenRetriever _bearerTokenRetriever;

	public RelativityAuthenticationProviderFactory(IConsoleLogger consoleLogger,
		IBearerTokenRetriever bearerTokenRetriever)
	{
		_consoleLogger = consoleLogger;
		_bearerTokenRetriever = bearerTokenRetriever;
	}

	public IRelativityAuthenticationProvider Create(CommonConfiguration common)
	{
		return new RelativityAuthenticationProvider(common, _consoleLogger, _bearerTokenRetriever);
	}
}