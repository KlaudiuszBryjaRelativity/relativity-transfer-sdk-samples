namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Threading.Tasks;
	using Relativity.Transfer.SDK.Interfaces;
	using Relativity.Transfer.SDK.Sample.Authentication.Credentials;
	using Relativity.Transfer.SDK.Interfaces.ProgressReporting;
	using Relativity.Transfer.SDK.Interfaces.Paths;
	using Relativity.Transfer.SDK.Core.ProgressReporting;
	using Relativity.Transfer.SDK.Sample.Authentication;
	using Relativity.Transfer.SDK.Sample.Monitoring;
	using Relativity.Transfer.SDK.Interfaces.Options;
	using Relativity.Transfer.SDK.Interfaces.Options.Policies;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Linq;
	using System.IO;

	public class Sample7_UploadDirectoryWithExclusionPolicy : SampleBase
	{
		public Sample7_UploadDirectoryWithExclusionPolicy(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public async override Task ExecuteAsync()
		{
			// get input values from config or ask user if not set in config 
			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			Guid transferId = Guid.NewGuid();

			DirectoryPath destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferId.ToString());

			// Create authentication provider 
			RelativityAuthenticationProvider authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new SecretCredentials(clientId, clientSecret));

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			// UploadDirectoryOptions with files exclusion policy
			UploadDirectoryOptions fileExclusionPolicyOptions = new UploadDirectoryOptions()
			{
				ExclusionPolicy = new FileNameAcceptExclusionPolicy(new[] { "*.xls", "*.doc", "*.txt" })	// creating policy that accepts only files with given extentions 
			};

			// create temporary directory with files which half fulfill the exlusion policy and half not 
			DirectoryPath sourcePath = CreateTemporaryDirectoryWithFiles(transferId.ToString(), "file1.xls", "file2.bin", "file3.doc", "file4.exe", "file5.txt", "file6.xml");

			Console.WriteLine($"Creating transfer {transferId} {Environment.NewLine}   - From {sourcePath} {Environment.NewLine}   - To {destinationPath}");

			// upload single file 
			TransferJobResult result = await transferClient
				.UploadDirectoryAsync(transferId, sourcePath, destinationPath, fileExclusionPolicyOptions, GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine($"Transfer {transferId} Finished with status {result.State.Status} {Environment.NewLine}   - Bytes transferred: {result.TotalBytes} {Environment.NewLine}   - Elapsed: {result.Elapsed} ");
		}

		private class FileNameAcceptExclusionPolicy : IFileExclusionPolicy
		{
			private readonly IEnumerable<Regex> _fileNamesToExclude;

			public FileNameAcceptExclusionPolicy(IEnumerable<string> fileNamesToAccept)
			{
				_fileNamesToExclude = fileNamesToAccept.Select( x => WildCardToRegular(x)).Select(x => new Regex(x));
			}

			public Task<bool> ShouldExcludeAsync(IFileReference fileReference)
			{
				foreach(Regex r in _fileNamesToExclude)
				{
					if(r.Match(fileReference.AbsolutePath).Success)
					{
						return Task.FromResult(false);
					}
				}
				return Task.FromResult(true);
			}

			private string WildCardToRegular(string value)	// converting wildcard "Like" convention to Regular expression
			{
				return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
			}
		}

		private DirectoryPath CreateTemporaryDirectoryWithFiles( string directoryName, params string[] files)
		{
			string sourceDirectoryPath = Path.Combine(Path.GetTempPath(), directoryName);
			Directory.CreateDirectory(sourceDirectoryPath);

			// create temporary source file to upload 
			foreach( string file in files) 
			{
				string sampleFile = Path.Combine(sourceDirectoryPath, file);
				using (StreamWriter sw = File.CreateText(sampleFile))
				{
					sw.WriteLine("This is transfer sdk example file");
				}
			}
			return new DirectoryPath(sourceDirectoryPath);
		}


		private static ITransferProgressHandler GetProgressHandler()
		{
			// Transfer progress is optional - you can subscribe to all, some, or no updates. Bear in mind though that
			// this is the only way you can obtain the information about eventual fails or skips on individual items and
			// that's why encourage the clients to take an advantage of it (subscribe and log it). TransferSDK don't stores
			// any information about failed file paths due to privacy concerns.
			return TransferProgressHandlerBuilder.Instance
				.OnStatistics(StatisticHook.OnStatisticsReceived) // Updates about overall status (% progress, transfer rates, partial statistics)
				.OnSucceededItem(StatisticHook.OnSucceededItemReceived) // Updates on each transferred item
				.OnFailedItem(StatisticHook.OnFailedItemReceived) // Updates on each failed item (and reason for it)
				.OnSkippedItem(StatisticHook.OnSkippedItemReceived) // Updates on each skipped item (and reason for it)
				.OnProgressSteps(StatisticHook.OnProgressStepsReceived) // Updates on each job's progress steps. Use it to track overall percentage progress and state.
				.Create();
		}
	}
}



