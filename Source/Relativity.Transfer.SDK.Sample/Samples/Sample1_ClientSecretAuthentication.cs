namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Collections.Generic;
	using System.Dynamic;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Net.Http;
	using Newtonsoft.Json;
	using Relativity.Transfer.SDK.Interfaces;
	using Relativity.Transfer.SDK.Interfaces.Authentication;
	using Relativity.Transfer.SDK.Interfaces.ProgressReporting;
	using Relativity.Transfer.SDK.Interfaces.Paths;

	public class Sample1_ClientSecretAuthentication : SampleBase
	{
		public Sample1_ClientSecretAuthentication( ConsoleHelper consoleHelper) : base( consoleHelper ) { }

		public async override Task ExecuteAsync()
		{
			// get input values from config or ask user if not set in config 
			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			Guid transferId = Guid.NewGuid();
			FilePath sourcePath = CreateTemporarySourceFile();
			DirectoryPath destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferId.ToString());

			// Create authentication provider 
			ClientSecretAuthenticationProvider authenticationProvider = new ClientSecretAuthenticationProvider(new Uri(relativityInstanceAddress), clientId, clientSecret);

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine($"Creating transfer {transferId} {Environment.NewLine}   - From {sourcePath} {Environment.NewLine}   - To {destinationPath}");

			// upload single file 
			TransferJobResult result = await transferClient
				.UploadFileAsync(transferId, sourcePath, destinationPath, default)
				.ConfigureAwait(false);

			Console.WriteLine($"Transfer {transferId} Finished with status {result.State.Status} {Environment.NewLine}   - Bytes transferred: {result.TotalBytes} {Environment.NewLine}   - Elapsed: {result.Elapsed} ");
		}

		// here is the implementation of authentication provider base on OAuth client id and client secret. 
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
					// we are caching the token here, but be aware that for long transfers, token can become depcrecated and we neet to ensure here that the token is valid
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
			// create temporary source file to upload 
			string sampleFile = Path.Combine(Path.GetTempPath(), "Sdk_Sample1_TemporaryFile.txt");
			using (StreamWriter sw = File.CreateText(sampleFile))
			{
				sw.WriteLine("This is Transfer SDK sample 1 temporary file");
			}
			return new FilePath(sampleFile);
		}
	}
}

