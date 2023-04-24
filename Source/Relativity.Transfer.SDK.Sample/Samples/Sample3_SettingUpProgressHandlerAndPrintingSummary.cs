namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Interfaces.ProgressReporting;
	using Interfaces.Paths;
	using Authentication;
	using Authentication.Credentials;
	using Core.ProgressReporting;
	using ByteSizeLib;
	using System.Text;
	using Helpers;

	internal class Sample3_SettingUpProgressHandlerAndPrintingSummary : SampleBase
	{
		public Sample3_SettingUpProgressHandlerAndPrintingSummary(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			const int smallFilesAmountToUpload = 100;
			const int largeFilesAmountToUpload = 2;
			const long largeFileSize = 200 * 1024 * 1024;

			Console.WriteLine($"This Sample will present statistics while sending {smallFilesAmountToUpload} small files and {largeFilesAmountToUpload} large files of {largeFileSize} Bytes each");
			Console.WriteLine();
			Console.WriteLine("Settings: ");

			var clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			var relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			var clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			var clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			var transferJobId = Guid.NewGuid();
			var sourcePath = CreateTemporaryDirectoryWithFiles(transferJobId.ToString(), smallFilesAmountToUpload, largeFilesAmountToUpload, largeFileSize);
			var destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			var authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new OAuthCredentials(clientId, clientSecret));

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			var transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			Console.WriteLine();
			Console.WriteLine($"Creating transfer \"{transferJobId}\" {Environment.NewLine}   - From:  {sourcePath} {Environment.NewLine}   - To:  {destinationPath}");
			Console.WriteLine();

			var result = await transferClient
				.UploadDirectoryAsync(transferJobId, sourcePath, destinationPath, GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer has finished: ");
			PrintTransferSummary(result);
		}
		private ITransferProgressHandler GetProgressHandler()
		{
			// Transfer progress is optional - you can subscribe to all, some, or no updates. Bear in mind though that this is the only way you can obtain information
			// about eventual fails or skips on individual items and that's why encourage the clients to take an advantage of it (subscribe and log it).
			// TransferSDK doesn't store any information about failed file paths due to privacy concerns.
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
			WriteLine($"  bytes transferred: {statistics.CurrentBytesTransferred} of {statistics.TotalBytes}", ConsoleColor.Blue);
		}

		private void PrintSucceededItem(TransferItemState itemState)
		{
			WriteLine($"  item transfer succeeded: {itemState.Source}", ConsoleColor.DarkGreen);
		}

		private void PrintFailedItem(TransferItemState itemState)
		{
			WriteLine($"  item transfer failed: {itemState.Source}", ConsoleColor.Red);
		}

		private void PrintSkippedItem(TransferItemState itemState)
		{
			WriteLine($"  item transfer skipped: {itemState.Source}", ConsoleColor.Yellow);
		}

		private void PrintProgressStep(IEnumerable<StepProgress> progressSteps)
		{
			WriteLine(string.Join(Environment.NewLine, progressSteps.Select(stepProgress => $"  Step name: {stepProgress.Name}, {stepProgress.PercentageProgress:F}%, State: {stepProgress.State}")), ConsoleColor.Cyan);
		}

		private void PrintTransferSummary(TransferJobResult result)
		{
			var sb = new StringBuilder();

			sb.AppendLine($"  - JobId: {result.CorrelationId}");
			sb.AppendLine($"  - Total Bytes: {result.TotalBytes} ({ByteSize.FromBytes(result.TotalBytes)})");
			sb.AppendLine($"  - Total Files Transferred: {result.TotalFilesTransferred}");
			sb.AppendLine($"  - Total Empty Directories Transferred: {result.TotalEmptyDirectoriesTransferred}");
			sb.AppendLine($"  - Total Files Skipped: {result.TotalFilesSkipped}");
			sb.AppendLine($"  - Total Files Failed: {result.TotalFilesFailed}");
			sb.AppendLine($"  - Total Empty Directories Failed: {result.TotalEmptyDirectoriesFailed}");
			sb.AppendLine($"  - Elapsed: {result.Elapsed:hh\\:mm\\:ss} s ({Math.Floor(result.Elapsed.TotalSeconds)} s)");
			sb.AppendLine($"  - Status: {result.State.Status}");

			WriteLine(sb.ToString(), ConsoleColor.Blue);
		}


		private static void WriteLine(string line, ConsoleColor color)
		{
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(line);
			Console.ForegroundColor = defaultColor;
		}

		private DirectoryPath CreateTemporaryDirectoryWithFiles(string directoryName, int smallFilesAmount, int largeFilesAmount, long largeFileSize )
		{
			var sampleDirectory = SourceFilesHelper.CreateDirectoryWithFiles(directoryName, smallFilesAmount, largeFilesAmount, largeFileSize);
			return new DirectoryPath(sampleDirectory);
		}
	}
}
