namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Relativity.Transfer.SDK.Interfaces;
	using Relativity.Transfer.SDK.Sample.Authentication.Credentials;
	using Relativity.Transfer.SDK.Interfaces.ProgressReporting;
	using Relativity.Transfer.SDK.Interfaces.Paths;
	using Relativity.Transfer.SDK.Core.ProgressReporting;
	using Relativity.Transfer.SDK.Sample.Authentication;

	public class Sample3_SettingUpProgressHandler : SampleBase
	{
		public Sample3_SettingUpProgressHandler(ConsoleHelper consoleHelper) : base(consoleHelper) { }

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
			RelativityAuthenticationProvider authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new SecretCredentials(clientId, clientSecret));

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine($"Creating transfer {transferId} {Environment.NewLine}   - From {sourcePath} {Environment.NewLine}   - To {destinationPath}");

			// upload single file 
			TransferJobResult result = await transferClient
				.UploadFileAsync(transferId, sourcePath, destinationPath, GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine($"Transfer {transferId} Finished with status {result.State.Status} {Environment.NewLine}   - Bytes transferred: {result.TotalBytes} {Environment.NewLine}   - Elapsed: {result.Elapsed} ");
		}


		private ITransferProgressHandler GetProgressHandler()
		{
			// Transfer progress is optional - you can subscribe to all, some, or no updates. Bear in mind though that
			// this is the only way you can obtain the information about eventual fails or skips on individual items and
			// that's why encourage the clients to take an advantage of it (subscribe and log it). TransferSDK don't stores
			// any information about failed file paths due to privacy concerns.
			return TransferProgressHandlerBuilder.Instance
				.OnStatistics(PrintStatistics) // Updates about overall status (% progress, transfer rates, partial statistics)
				.OnSucceededItem(PrintSucceededItem) // Updates on each transferred item
				.OnFailedItem(PrintFailedItem) // Updates on each failed item (and reason for it)
				.OnSkippedItem(PrintSkippedItem) // Updates on each skipped item (and reason for it)
				.OnProgressSteps(PrintProgressStep) // Updates on each job's progress steps. Use it to track overall percentage progress and state.
				.Create();
		}

		private void PrintStatistics( TransferJobStatistics statistics)
		{
			Console.WriteLine($"  bytes transferred: {statistics.CurrentBytesTransferred} of {statistics.TotalBytes}");
		}

		private void PrintSucceededItem(TransferItemState itemState)
		{
			Console.WriteLine($"  item transfer succeeded: {itemState.Source}");
		}

		private void PrintFailedItem(TransferItemState itemState)
		{
			Console.WriteLine($"  item transfer failed: {itemState.Source}");
		}

		private void PrintSkippedItem(TransferItemState itemState)
		{
			Console.WriteLine($"  item transfer skipped: {itemState.Source}");
		}

		private void PrintProgressStep(IEnumerable<StepProgress> progressSteps)
		{
			Console.WriteLine(string.Join(Environment.NewLine,
				progressSteps.Select(stepProgress => $"  Step name: {stepProgress.Name}, {stepProgress.PercentageProgress:F}%, State: {stepProgress.State}")));
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


