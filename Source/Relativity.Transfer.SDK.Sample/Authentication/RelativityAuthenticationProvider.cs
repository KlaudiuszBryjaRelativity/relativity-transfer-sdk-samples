namespace Relativity.Transfer.SDK.Sample.Authentication
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Interfaces.Authentication;
	using Credentials;

	internal class RelativityAuthenticationProvider : IRelativityAuthenticationProvider
	{
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

		public Uri BaseAddress { get; }

		public async Task<RelativityCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(_bearerToken))
				{
					return new RelativityCredentials(_bearerToken, BaseAddress);
				}

				Console.WriteLine("Retrieving bearer token...");
				_bearerToken = await _bearerTokenRetriever.RetrieveTokenAsync(_secretCredentials.ClientId, _secretCredentials.ClientSecret).ConfigureAwait(false);

				return new RelativityCredentials(_bearerToken, BaseAddress);
			}
			catch( Exception e) 
			{
				throw new ApplicationException("Failed to retrieve credentials.", e);
			}
		}
	}
}
