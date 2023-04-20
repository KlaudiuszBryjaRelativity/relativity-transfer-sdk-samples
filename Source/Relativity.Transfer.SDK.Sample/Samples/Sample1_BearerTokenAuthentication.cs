namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Dynamic;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Net.Http;
	using Newtonsoft.Json;
	using Interfaces;
	using Interfaces.ProgressReporting;
	using Interfaces.Paths;
	using Interfaces.Authentication;
	using Helpers;

	internal class Sample1_BearerTokenAuthentication : SampleBase
	{
		public Sample1_BearerTokenAuthentication(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			Guid transferJobId = Guid.NewGuid();
			FilePath sourcePath = CreateTemporarySourceFile();
			DirectoryPath destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			ClientSecretAuthenticationProvider authenticationProvider = new ClientSecretAuthenticationProvider(new Uri(relativityInstanceAddress), clientId, clientSecret);

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (besides the client name)
			// is the authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine();
			Console.WriteLine($"Creating transfer \"{transferJobId}\" {Environment.NewLine}   - From:  {sourcePath} {Environment.NewLine}   - To:  {destinationPath}");
			Console.WriteLine();

			TransferJobResult result = await transferClient
				.UploadFileAsync(transferJobId, sourcePath, destinationPath, default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer {transferJobId} Finished with status {result.State.Status} {Environment.NewLine}   - Bytes transferred: {result.TotalBytes} {Environment.NewLine}   - Elapsed: {result.Elapsed} ");
		}

		// Implementation of Authentication Provider based on OAuth client id and client secret.
		private class ClientSecretAuthenticationProvider : IRelativityAuthenticationProvider
		{
			private readonly string _clientSecret;
			private readonly string _clientId;
			private string _bearerToken;

			public ClientSecretAuthenticationProvider(Uri baseUri, string clientId, string clientSecret)
			{
				BaseAddress = baseUri;
				_clientId = clientId;
				_clientSecret = clientSecret;
			}

			public Uri BaseAddress { get; }

			public async Task<RelativityCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
			{
				if(!string.IsNullOrWhiteSpace(_bearerToken))
				{
					Console.WriteLine("Authentication provider - Requesting credentials (CACHED).. ");
					return new RelativityCredentials(_bearerToken, BaseAddress);
				} 
				else
				{
					Console.WriteLine("Authentication provider - Requesting credentials.. ");
					// The token is cached by TransferSDK, but be aware it may expire during long transfers. That's why your implementation should not cache the token and always provide a new one.
					_bearerToken = await GetBearerTokenAsync(_clientSecret);
					return new RelativityCredentials(_bearerToken, BaseAddress);
				}
			}

			private async Task<string> GetBearerTokenAsync(string clientSecret)
			{
				Console.WriteLine("Authentication provider - Getting bearer token.. ");

				const string IdentityServiceTokenUri = "/Relativity/Identity/connect/token";

				HttpClient httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");    // default security header 

				Uri url = new Uri(BaseAddress, IdentityServiceTokenUri);
				var payload = new Dictionary<string, string>
				{
					{ "client_id", _clientId },
					{ "client_secret", clientSecret },
					{ "scope", "SystemUserInfo" },
					{ "grant_type", "client_credentials" }
				};

				using (HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(payload)))
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
		}

		private FilePath CreateTemporarySourceFile()
		{
			return new FilePath(SourceFilesHelper.CreateFile("Sdk_Sample1_TemporaryFile.txt"));
		}
	}
}
