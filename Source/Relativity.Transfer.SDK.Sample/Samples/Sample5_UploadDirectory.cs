namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Threading.Tasks;
	using Authentication;
	using Authentication.Credentials;
	using Monitoring;
	using Helpers;

	internal class Sample5_UploadDirectory : SampleBase
	{
		public Sample5_UploadDirectory(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			var clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			var relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			var clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			var clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			var transferJobId = Guid.NewGuid();
			var sourcePath = _consoleHelper.EnterSourceDirectoryPathOrTakeDefault();
			var destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			var authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new OAuthCredentials(clientId, clientSecret));

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
				.UploadDirectoryAsync(transferJobId, sourcePath, destinationPath, ConsoleStatisticHook.GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer has finished: ");
			Console.WriteLine(new TransferJobSummary(result));
		}
	}
}
