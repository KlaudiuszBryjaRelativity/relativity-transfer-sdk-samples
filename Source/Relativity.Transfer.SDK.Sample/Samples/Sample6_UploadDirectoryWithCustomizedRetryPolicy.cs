namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Threading.Tasks;
	using Interfaces;
	using Interfaces.ProgressReporting;
	using Interfaces.Paths;
	using Interfaces.Options;
	using Interfaces.Options.Policies;
	using Authentication;
	using Authentication.Credentials;
	using Monitoring;
	using Helpers;

	internal class Sample6_UploadDirectoryWithCustomizedRetryPolicy : SampleBase
	{
		public Sample6_UploadDirectoryWithCustomizedRetryPolicy(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			Guid transferJobId = Guid.NewGuid();
			DirectoryPath sourcePath = _consoleHelper.EnterSourceDirectoryPathOrTakeDefault();
			DirectoryPath destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			RelativityAuthenticationProvider authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new OAuthCredentials(clientId, clientSecret));

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is the authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine();
			Console.WriteLine($"Creating transfer \"{transferJobId}\" {Environment.NewLine}   - From:  {sourcePath} {Environment.NewLine}   - To:  {destinationPath}");
			Console.WriteLine();

			UploadDirectoryOptions exponentialRetryPolicyOptions = new UploadDirectoryOptions
			{
				TransferRetryPolicyDefinition = TransferRetryPolicyDefinition.ExponentialPolicy(TimeSpan.FromSeconds(2), 2)
			};

			TransferJobResult result = await transferClient
				.UploadDirectoryAsync(transferJobId, sourcePath, destinationPath, exponentialRetryPolicyOptions, ConsoleStatisticHook.GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer has finished: ");
			Console.WriteLine(new TransferJobSummary(result));
		}
	}
}
