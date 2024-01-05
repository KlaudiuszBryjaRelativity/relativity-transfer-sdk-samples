using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Relativity.Transfer.SDK.Samples.Core.Authentication.Credentials;

namespace Relativity.Transfer.SDK.Samples.Core.Authentication;

// More info: https://platform.relativity.com/RelativityOne/Content/REST_API/REST_API_authentication.htm#_Bearer_token_authentication
internal class BearerTokenRetriever : IBearerTokenRetriever
{
	private const string IdentityServiceTokenUri = "/Relativity/Identity/connect/token";

	private readonly HttpClient _httpClient;

	public BearerTokenRetriever()
	{
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
	}

	public async Task<string> RetrieveTokenAsync(Uri baseUri, OAuthCredentials oAuthCredentials)
	{
		try
		{
			var url = new Uri(baseUri, IdentityServiceTokenUri);
			var payload = new Dictionary<string, string>
			{
				{ "client_id", oAuthCredentials.ClientId },
				{ "client_secret", oAuthCredentials.ClientSecret },
				{ "scope", "SystemUserInfo" },
				{ "grant_type", "client_credentials" }
			};

			using var response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(payload));
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				throw new InvalidOperationException(
					$"API call {IdentityServiceTokenUri} failed with status code: '{response.StatusCode}' Details: '{content}'.");
			}

			var contentString = await response.Content.ReadAsStringAsync();
			dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(contentString);

			return data.access_token;
		}
		catch (Exception e)
		{
			throw new ApplicationException("Failed to retrieve bearer token.", e);
		}
	}
}