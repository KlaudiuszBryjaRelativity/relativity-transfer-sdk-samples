using System;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Samples.Core.Authentication.Credentials;

namespace Relativity.Transfer.SDK.Samples.Core.Authentication;

internal interface IBearerTokenRetriever
{
	Task<string> RetrieveTokenAsync(Uri baseUri, OAuthCredentials oAuthCredentials);
}