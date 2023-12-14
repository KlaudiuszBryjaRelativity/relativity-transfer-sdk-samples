using Relativity.Transfer.SDK.Interfaces.Authentication;
using Relativity.Transfer.SDK.Sample.Configuration;

namespace Relativity.Transfer.SDK.Sample.Authentication;

internal interface IRelativityAuthenticationProviderFactory
{
	IRelativityAuthenticationProvider Create(CommonConfiguration common);
}