using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Relativity.Transfer.SDK.Interfaces.Authentication;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Samples.Core.Attributes;
using Relativity.Transfer.SDK.Samples.Core.Authentication.Credentials;
using Relativity.Transfer.SDK.Samples.Core.Configuration;
using Relativity.Transfer.SDK.Samples.Core.Helpers;
using Relativity.Transfer.SDK.Samples.Core.Runner;

namespace Relativity.Transfer.SDK.Samples.Repository.FullPathWorkflow;

[Sample(1, "A bearer token authentication",
	"The sample illustrates how to implement a bearer token authentication in RelativityOne.",
	typeof(BearerTokenAuthentication),
	TransferType.UploadFile)]
internal class BearerTokenAuthentication(
	IConsoleLogger consoleLogger,
	IPathExtension pathExtension)
	: ISample
{
	public async Task ExecuteAsync(Configuration configuration, CancellationToken token)
	{
		var clientName = configuration.Common.ClientName;
		var jobId = configuration.Common.JobId;
		var source = string.IsNullOrWhiteSpace(configuration.UploadFile.Source)
			? new DisposablePath<FilePath>(pathExtension.CreateTemporarySourceFile()).Path
			: new FilePath(configuration.UploadFile.Source);
		var destination = string.IsNullOrWhiteSpace(configuration.UploadFile.Destination)
			? pathExtension.GetDefaultRemoteDirectoryPathForUpload(configuration.Common)
			: new DirectoryPath(configuration.UploadFile.Destination);
		// Instance of the authentication provider that will be used by the TransferClient.
		var authenticationProvider = new ClientSecretAuthenticationProvider(consoleLogger,
			new Uri(configuration.Common.InstanceUrl),
			configuration.Common.OAuthCredentials);

		// The builder follows the Fluent convention, and more options will be added in the future. The only required component (besides the client name)
		// is the authentication provider - a provided one that utilizes an OAuth-based approach has been provided, but the custom implementation can be created.
		var transferClient = TransferClientBuilder.FullPathWorkflow
			.WithAuthentication(authenticationProvider)
			.WithClientName(clientName)
			.Build();

		consoleLogger.PrintCreatingTransfer(jobId, source, destination);

		var result = await transferClient
			.UploadFileAsync(jobId, source, destination, token)
			.ConfigureAwait(false);

		consoleLogger.PrintTransferResult(result);
	}

	/// <summary>
	///     This class represents an implementation of an Authentication Provider based on OAuth client id and client secret.
	///     Be aware that this is sample implementation, and it should be used only for testing purposes.
	/// </summary>
	private class ClientSecretAuthenticationProvider(
		IConsoleLogger consoleLogger,
		Uri instanceUri,
		OAuthCredentials credentials)
		: IRelativityAuthenticationProvider
	{
		private string _bearerToken;

		public Uri BaseAddress { get; } = instanceUri;

		public async Task<RelativityCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
		{
			if (!string.IsNullOrWhiteSpace(_bearerToken))
			{
				consoleLogger.Info("Authentication provider - Requesting credentials ([green]CACHED[/])");

				return new RelativityCredentials(_bearerToken, BaseAddress);
			}

			consoleLogger.Info("Authentication provider - [green]Requesting credentials[/]...");
			// The token is cached by TransferSDK, but it is important to note that a token may expire during long transfers.
			// Therefore, it is imperative that your implementation does not cache the token and always provides a valid one.
			_bearerToken = await GetBearerTokenAsync().ConfigureAwait(false);

			return new RelativityCredentials(_bearerToken, BaseAddress);
		}

		/// <summary>
		///     The README.md file contains references that can assist in creating a valid OAuth flow.
		/// </summary>
		private async Task<string> GetBearerTokenAsync()
		{
			Console.WriteLine("Authentication provider - Getting bearer token.. ");

			const string identityServiceTokenUri = "/Relativity/Identity/connect/token";

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-"); // Default security header

			var url = new Uri(BaseAddress, identityServiceTokenUri);
			var payload = new Dictionary<string, string>
			{
				{ "client_id", credentials.ClientId },
				{ "client_secret", credentials.ClientSecret },
				{ "scope", "SystemUserInfo" },
				{ "grant_type", "client_credentials" }
			};

			using var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(payload));

			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				throw new InvalidOperationException(
					$"API call {identityServiceTokenUri} failed with status code: '{response.StatusCode}' Details: '{content}'.");
			}

			var contentString = await response.Content.ReadAsStringAsync();
			dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(contentString);

			return data.access_token;
		}
	}
}