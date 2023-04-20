namespace Relativity.Transfer.SDK.Sample.Authentication.Credentials
{
	internal class OAuthCredentials
	{
		public OAuthCredentials(string clientId, string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
		}

		public string ClientId { get; }
		public string ClientSecret { get; }
	}
}
