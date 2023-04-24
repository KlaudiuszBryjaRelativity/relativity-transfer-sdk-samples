namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Dynamic;
	using System.Text;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Net.Http;
	using Newtonsoft.Json;
	using Interfaces.Authentication;
	using Interfaces.Paths;
	using Helpers;

	internal class Sample2_BasicCredentialsAndBearerAuthentication : SampleBase
	{
		public Sample2_BasicCredentialsAndBearerAuthentication(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			var clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			var relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			var userLogin = _consoleHelper.GetOrEnterSetting(SettingNames.ClientLogin);
			var userPassword = _consoleHelper.GetOrEnterSetting(SettingNames.ClientPassword);
			var base64BasicUserCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userLogin + ":" + userPassword));
			var oauthClientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			var transferJobId = Guid.NewGuid();
			var sourcePath = CreateTemporarySourceFile();
			var destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			var authenticationProvider = new BasicCredentialsAuthenticationProvider(new Uri(relativityInstanceAddress), oauthClientId, base64BasicUserCredentials);

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is the authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			var transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine();
			Console.WriteLine($"Creating transfer \"{transferJobId}\" {Environment.NewLine}   - From:  {sourcePath} {Environment.NewLine}   - To:  {destinationPath}");
			Console.WriteLine();

			var result = await transferClient
				.UploadFileAsync(transferJobId, sourcePath, destinationPath, default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer {transferJobId} Finished with status {result.State.Status} {Environment.NewLine}   - Bytes transferred: {result.TotalBytes} {Environment.NewLine}   - Elapsed: {result.Elapsed} ");
		}

		private class BasicCredentialsAuthenticationProvider : IRelativityAuthenticationProvider
		{
			private readonly string _oAuthClientId;
			private readonly string _base64BasicUserCredentialsHeader;

			private string _bearerToken;

			public BasicCredentialsAuthenticationProvider(Uri baseUri, string oauthClientId, string base64EncodedCredentials)
			{
				BaseAddress = baseUri;
				_oAuthClientId = oauthClientId;
				_base64BasicUserCredentialsHeader = $"Basic {base64EncodedCredentials}";
			}

			public Uri BaseAddress { get; }

			public async Task<RelativityCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
			{
				if (!string.IsNullOrWhiteSpace(_bearerToken))
				{
					Console.WriteLine("Authentication provider - Requesting credentials (CACHED).. ");
					return new RelativityCredentials(_bearerToken, BaseAddress);
				}
				else
				{
					Console.WriteLine("Authentication provider - Requesting credentials.. ");
					var newSecret = await GetOauth2ClientSecretAsync();
					newSecret = newSecret.Trim('"');

					_bearerToken = await GetBearerTokenAsync(newSecret);

					return new RelativityCredentials(_bearerToken, BaseAddress);

				}
			}

			private async Task<string> GetOauth2ClientSecretAsync()
			{
				Console.WriteLine("Authentication provider - Getting OAuth2 client secret.. ");

				const string MediaTypeApplicationJson = "application/json";
				const string OAuth2ClientManagerUri = "/Relativity.Rest/api/Relativity.Services.Security.ISecurityModule/OAuth2 Client Manager/";
				//const string MethodName = "RegenerateSecretAsync";// this method will create completely new secret for the user 
				
				// this method will read current client data, including secret.
				// In real scenario it is strongly recommended to use the commented method above, which will regenerated the secret at the same time.
				// But here we use current method to not spoil the secret for other samples which are based on secret stored at the beginning 
				const string MethodName = "ReadAsync";


				var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");    // default security header 
				httpClient.DefaultRequestHeaders.Add("Authorization", _base64BasicUserCredentialsHeader);    // Authorization header 

				var managerUri = new Uri(BaseAddress, OAuth2ClientManagerUri);
				var url = new Uri(managerUri, MethodName);
				var body = new StringContent($"{{id:'{_oAuthClientId}'}}", Encoding.UTF8, MediaTypeApplicationJson);

				using (HttpResponseMessage response = await httpClient.PostAsync(url, body))
				{
					if (response.IsSuccessStatusCode)
					{
						var contentString = await response.Content.ReadAsStringAsync();
						dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(contentString);
						return data.Secret;

					}
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
					throw new InvalidOperationException($"API call '{MethodName}' failed with status code: '{response.StatusCode}' Details: '{content}'.");
				}
			}

			private async Task<string> GetBearerTokenAsync(string clientSecret)
			{
				Console.WriteLine("Authentication provider - Getting bearer token.. ");

				const string IdentityServiceTokenUri = "/Relativity/Identity/connect/token";

				var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");    // default security header 

				var url = new Uri(BaseAddress, IdentityServiceTokenUri);
				var payload = new Dictionary<string, string>
				{
					{ "client_id", _oAuthClientId },
					{ "client_secret", clientSecret },
					{ "scope", "SystemUserInfo" },
					{ "grant_type", "client_credentials" }
				};

				using (HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(payload)))
				{
					if (!response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
						throw new InvalidOperationException($"API call {IdentityServiceTokenUri} failed with status code: '{response.StatusCode}' Details: '{content}'.");
					}
					var contentString = await response.Content.ReadAsStringAsync();
					dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(contentString);
					return data.access_token;
				}
			}
		}

		private FilePath CreateTemporarySourceFile()
		{
			return new FilePath(SourceFilesHelper.CreateFile("Sdk_Sample2_TemporaryFile.txt"));
		}
	}
}
