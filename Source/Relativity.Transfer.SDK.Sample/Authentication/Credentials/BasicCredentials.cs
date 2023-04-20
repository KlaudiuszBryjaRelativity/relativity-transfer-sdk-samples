namespace Relativity.Transfer.SDK.Sample.Authentication.Credentials
{
	using System;
	using System.Text;

	internal class BasicCredentials
	{
		public BasicCredentials(string login, string password, string oauthClientId)
		{
			Base64EncodedBasicCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(login + ":" + password));
			OAuthClientId = oauthClientId;
		}
		public string Base64EncodedBasicCredentials { get; }
		public string OAuthClientId { get; }
	}
}
