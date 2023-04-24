namespace Relativity.Transfer.SDK.Sample.Authentication
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Interfaces.Authentication;
	using Credentials;

	internal class RelativityAuthenticationProvider : IRelativityAuthenticationProvider
	{
		private readonly OAuthClientManager _oAuthManager;
		private readonly BearerTokenRetriever _bearerTokenRetriever;
		private readonly OAuthCredentials _secretCredentials;
		private readonly BasicCredentials _basicCredentials;

		private string _bearerToken;

		public RelativityAuthenticationProvider(string relativityInstanceUrl, OAuthCredentials secretCredentials)
		{
			if( string.IsNullOrWhiteSpace(relativityInstanceUrl) || null == secretCredentials || string.IsNullOrWhiteSpace(secretCredentials.ClientId) || string.IsNullOrWhiteSpace(secretCredentials.ClientSecret))
			{
				throw new ArgumentException("Invalid Authentication arguments");
			}
			BaseAddress = new Uri(relativityInstanceUrl);
			_secretCredentials = secretCredentials;
			_bearerTokenRetriever = new BearerTokenRetriever(relativityInstanceUrl);
		}

		public RelativityAuthenticationProvider(string relativityInstanceUrl, BasicCredentials basicCredentials)
		{
			if (string.IsNullOrWhiteSpace(relativityInstanceUrl) || null == basicCredentials || string.IsNullOrWhiteSpace(basicCredentials.OAuthClientId) || string.IsNullOrWhiteSpace(basicCredentials.Base64EncodedBasicCredentials))
			{
				throw new ArgumentException("Invalid Authentication arguments");
			}
			BaseAddress = new Uri(relativityInstanceUrl);
			_basicCredentials = basicCredentials;
			_oAuthManager = new OAuthClientManager(relativityInstanceUrl, basicCredentials.OAuthClientId, basicCredentials.Base64EncodedBasicCredentials);
			_bearerTokenRetriever = new BearerTokenRetriever(relativityInstanceUrl);
		}

		public Uri BaseAddress { get; }

		public async Task<RelativityCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(_bearerToken))
				{
					return new RelativityCredentials(_bearerToken, BaseAddress);
				}

				string clientId;
				string clientSecret;
				if (_secretCredentials == null)
				{
					clientId = _basicCredentials.OAuthClientId;
					Console.WriteLine("Retrieving client secret...");
					clientSecret = await _oAuthManager.RetrieveClientSecretAsync();
				}
				else
				{
					clientId = _secretCredentials.ClientId;
					clientSecret = _secretCredentials.ClientSecret;
				}

				Console.WriteLine("Retrieving bearer token...");
				_bearerToken = await _bearerTokenRetriever.RetrieveTokenAsync(clientId, clientSecret).ConfigureAwait(false);

				return new RelativityCredentials(_bearerToken, BaseAddress);
			}
			catch( Exception e) 
			{
				throw new ApplicationException("Failed to retrieve credentials.", e);
			}
		}
	}
}
