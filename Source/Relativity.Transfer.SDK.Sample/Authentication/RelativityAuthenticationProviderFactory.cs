using Relativity.Transfer.SDK.Interfaces.Authentication;
using Relativity.Transfer.SDK.Sample.Configuration;
using Relativity.Transfer.SDK.Sample.Helpers;

namespace Relativity.Transfer.SDK.Sample.Authentication;

internal sealed class RelativityAuthenticationProviderFactory : IRelativityAuthenticationProviderFactory
{
	private readonly IBearerTokenRetriever _bearerTokenRetriever;
	private readonly IConsoleLogger _consoleLogger;

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