namespace Relativity.Transfer.SDK.Sample.Authentication
{
	using System;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;

	// More info: https://platform.relativity.com/10.3/Content/Authentication/OAuth2_clients.htm#_OAuth2_Client_Manager_REST_service
	internal class OAuthClientManager
	{
		private const string MediaTypeApplicationJson = "application/json";
		private const string OAuth2ClientManagerUri = "/Relativity.Rest/api/Relativity.Services.Security.ISecurityModule/OAuth2 Client Manager/";
		private const string RegenerateSecretMethodName = "RegenerateSecretAsync"; // this will regenerate and return the secret 
		private const string ReadMethodName = "ReadAsync";	// this only reads outh client data ( which means secret can be outdated) 

		private readonly HttpClient _httpClient;
		private readonly string _oAuthClientId;
		private readonly Uri _baseUri;

		public OAuthClientManager(string relativityInstanceUrl, string oauthClientId, string base64BasicCredentials)
		{
			_baseUri = new Uri(new Uri(relativityInstanceUrl), OAuth2ClientManagerUri);
			_oAuthClientId = oauthClientId;
			_httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64BasicCredentials}");
		}

		public async Task<string> RetrieveClientSecretAsync()
		{
			try
			{
				HttpContent body = new StringContent($"{{id:'{_oAuthClientId}'}}", Encoding.UTF8, MediaTypeApplicationJson);
				using (HttpResponseMessage response = await PostAsync(RegenerateSecretMethodName, body))
				{
					string secret = await ParsePlainStringResponse(response);
					return secret;
				}
			}
			catch (Exception e)
			{
				throw new ApplicationException("Failed to retrieve client secret.", e);
			}
		}

		private async Task<HttpResponseMessage> PostAsync(string method, HttpContent body)
		{
			var url = new Uri(_baseUri, method).ToString();
			HttpResponseMessage response = await _httpClient.PostAsync(url, body);
			if (response.IsSuccessStatusCode)
			{
				return response;
			}
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			throw new Exception($"API call '{method}' failed with status code: '{response.StatusCode}' Details: '{content}'.");
		}

		private async Task<string> ParsePlainStringResponse(HttpResponseMessage response)
		{
			return (await response.Content.ReadAsStringAsync()).Trim('"');
		}
	}
}
