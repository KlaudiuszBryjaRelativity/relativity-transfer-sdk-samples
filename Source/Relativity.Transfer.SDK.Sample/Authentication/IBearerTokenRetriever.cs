using System;
using System.Threading.Tasks;
using Relativity.Transfer.SDK.Sample.Authentication.Credentials;

namespace Relativity.Transfer.SDK.Sample.Authentication;

internal interface IBearerTokenRetriever
{
	Task<string> RetrieveTokenAsync(Uri baseUri, OAuthCredentials oAuthCredentials);
}