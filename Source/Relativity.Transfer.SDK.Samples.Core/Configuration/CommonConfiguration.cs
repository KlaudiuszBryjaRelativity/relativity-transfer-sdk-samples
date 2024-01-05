using System;
using Relativity.Transfer.SDK.Samples.Core.Authentication.Credentials;

namespace Relativity.Transfer.SDK.Samples.Core.Configuration;

internal sealed record CommonConfiguration(
	string ClientName,
	string InstanceUrl,
	string FileShareRoot,
	string FileShareRelativePath,
	OAuthCredentials OAuthCredentials)
{
	internal Guid JobId { get; set; }

	public void Deconstruct(out string clientName, out string instanceUrl, out string fileShareRoot,
		out string fileShareRelativePath, out OAuthCredentials oauthCredentials)
	{
		clientName = ClientName;
		instanceUrl = InstanceUrl;
		fileShareRoot = FileShareRoot;
		fileShareRelativePath = FileShareRelativePath;
		oauthCredentials = OAuthCredentials;
	}
}