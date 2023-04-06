namespace Relativity.Transfer.SDK.Sample.Authentication
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.Net.Http;
	using System.Threading.Tasks;

	internal class BearerTokenRetriever
	{
		private const string IdentityServiceTokenUri = "/Relativity/Identity/connect/token";

		private readonly Uri _baseUri;
		private readonly HttpClient _httpClient;

		public BearerTokenRetriever(string relativityInstanceUrl)
		{
			_baseUri = new Uri(relativityInstanceUrl);
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
		}

		public async Task<string> RetrieveTokenAsync(string clientId, string clientSecret)
		{
			try
			{
				Uri url = new Uri(_baseUri, IdentityServiceTokenUri);
				var payload = new Dictionary<string, string>
				{
					{ "client_id", clientId },
					{ "client_secret", clientSecret },
					{ "scope", "SystemUserInfo" },
					{ "grant_type", "client_credentials" }
				};

				using (HttpResponseMessage response = await _httpClient.PostAsync(url, new FormUrlEncodedContent(payload)))
				{
					if (!response.IsSuccessStatusCode)
					{
						string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						throw new InvalidOperationException($"API call {IdentityServiceTokenUri} failed with status code: '{response.StatusCode}' Details: '{content}'.");
					}
					string contentString = await response.Content.ReadAsStringAsync();
					dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(contentString);
					return data.access_token;
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException("Failed to retrieve bearer token.", e);
			}
		}
	}
}
