using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Interfaces.Authentication;

namespace Relativity.Transfer.SDK.Samples.Core.Authentication;

internal interface IRelativityAuthenticationProviderFactory
{
	IRelativityAuthenticationProvider Create(CommonConfiguration common);
}