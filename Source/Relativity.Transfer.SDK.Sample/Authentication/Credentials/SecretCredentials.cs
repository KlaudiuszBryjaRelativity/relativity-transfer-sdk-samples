namespace Relativity.Transfer.SDK.Sample.Authentication.Credentials
{
	public class SecretCredentials
	{
		public SecretCredentials(string oauthClientId, string oauthClientSecret)
		{
			OAuthClientId = oauthClientId;
			OAuthClientSecret = oauthClientSecret;
		}

		public string OAuthClientId { get; private set; }
		public string OAuthClientSecret { get; private set; }
	}
}
