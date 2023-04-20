namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using System.Linq;
	using Interfaces;
	using Interfaces.ProgressReporting;
	using Interfaces.Paths;
	using Interfaces.Options;
	using Interfaces.Options.Policies;
	using Authentication;
	using Authentication.Credentials;
	using Monitoring;
	using Helpers;

	internal class Sample7_UploadDirectoryWithExclusionPolicy : SampleBase
	{
		public Sample7_UploadDirectoryWithExclusionPolicy(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			Guid transferJobId = Guid.NewGuid();

			DirectoryPath destinationPath = _consoleHelper.GetDestinationDirectoryPath(transferJobId.ToString());

			RelativityAuthenticationProvider authenticationProvider = new RelativityAuthenticationProvider(relativityInstanceAddress, new OAuthCredentials(clientId, clientSecret));

			// Builder follows Fluent convention, we'll add more options in the future. The only required component (beside client name)
			// is the authentication provider - we have provided one that utilizes OAuth based approach, but you can create your own.
			ITransferFullPathClient transferClient = TransferClientBuilder.Instance
				.WithAuthentication(authenticationProvider)
				.WithClientName(clientName)
				.Build();

			// A policy that accepts only *.xls, *.doc, and *.txt files 
			UploadDirectoryOptions fileExclusionPolicyOptions = new UploadDirectoryOptions()
			{
				ExclusionPolicy = new FileNameAcceptExclusionPolicy(new[] { "*.xls", "*.doc", "*.txt" })
			};

			// A dataset with files that will partially get excluded by the policy.
			DirectoryPath sourcePath = CreateTemporaryDirectoryWithFiles(transferJobId.ToString(), "file1.xls", "file2.bin", "file3.doc", "file4.exe", "file5.txt", "file6.xml");

			Console.WriteLine();
			Console.WriteLine($"Creating transfer \"{transferJobId}\" {Environment.NewLine}   - From:  {sourcePath} {Environment.NewLine}   - To:  {destinationPath}");
			Console.WriteLine();

			TransferJobResult result = await transferClient
				.UploadDirectoryAsync(transferJobId, sourcePath, destinationPath, fileExclusionPolicyOptions, ConsoleStatisticHook.GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer has finished: ");
			Console.WriteLine(new TransferJobSummary(result));
		}

		private class FileNameAcceptExclusionPolicy : IFileExclusionPolicy
		{
			private readonly IEnumerable<Regex> _fileNamesToExclude;

			public FileNameAcceptExclusionPolicy(IEnumerable<string> fileNamesToAccept)
			{
				_fileNamesToExclude = fileNamesToAccept.Select(WildCardToRegular).Select(x => new Regex(x));
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

		private static DirectoryPath CreateTemporaryDirectoryWithFiles(string directoryName, params string[] files)
		{
			return new DirectoryPath(SourceFilesHelper.CreateDirectoryWithFiles(directoryName, files));
		}
	}
}
