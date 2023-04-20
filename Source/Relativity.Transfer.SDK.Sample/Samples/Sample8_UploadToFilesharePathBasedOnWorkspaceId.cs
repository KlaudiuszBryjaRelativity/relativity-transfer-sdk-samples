namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Text;
	using System.Dynamic;
	using System.Linq;
	using System.IO;
	using Newtonsoft.Json;
	using Interfaces;
	using Interfaces.ProgressReporting;
	using Interfaces.Paths;
	using Authentication;
	using Authentication.Credentials;
	using Monitoring;
	using Helpers;

	internal class Sample8_UploadToFilesharePathBasedOnWorkspaceId : SampleBase
	{
		public Sample8_UploadToFilesharePathBasedOnWorkspaceId(ConsoleHelper consoleHelper) : base(consoleHelper) { }

		public override async Task ExecuteAsync()
		{
			Console.WriteLine("Settings: ");

			string clientName = _consoleHelper.GetOrEnterSetting(SettingNames.ClientName);
			string relativityInstanceAddress = _consoleHelper.GetOrEnterSetting(SettingNames.RelativityOneInstanceUrl);
			string clientId = _consoleHelper.GetOrEnterSetting(SettingNames.ClientOAuth2Id);
			string clientSecret = _consoleHelper.GetOrEnterSetting(SettingNames.ClientSecret);
			string clientLogin = _consoleHelper.GetOrEnterSetting(SettingNames.ClientLogin);
			string clientPassword = _consoleHelper.GetOrEnterSetting(SettingNames.ClientPassword);

			// provide workspace id 
			string workspaceIdStr = _consoleHelper.EnterUntilValid("Enter workspaceId to list its fileshares.", "[-]*[0-9]+");
			int workspaceId = int.Parse(workspaceIdStr);

			// Get list of fileshares associated with the provided workspace.The association is based on Resource Pool assigned to the workspace.
			WorkspaceFilesharesRetriever filesharesRetriever = new WorkspaceFilesharesRetriever(relativityInstanceAddress);
			IEnumerable<FileshareInfo> fileshares = await filesharesRetriever.GetWorkspaceFilesharesAsync(workspaceId, clientLogin, clientPassword).ConfigureAwait(false);

			if (!fileshares.Any())
			{
				Console.WriteLine("  There are no fileshares for provided workspaceId!");
				return;
			}

			FileshareInfo choosenFileshare = ChooseFileshare(fileshares);

			DirectoryPath sourcePath = _consoleHelper.EnterSourceDirectoryPathOrTakeDefault();

			Guid transferJobId = Guid.NewGuid();
			DirectoryPath destinationPath = GetDestinationPath(choosenFileshare.UncPath, transferJobId.ToString());

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

			TransferJobResult result = await transferClient
				.UploadDirectoryAsync(transferJobId, sourcePath, destinationPath, ConsoleStatisticHook.GetProgressHandler(), default)
				.ConfigureAwait(false);

			Console.WriteLine();
			Console.WriteLine($"Transfer has finished: ");
			Console.WriteLine(new TransferJobSummary(result));
		}

		private class FileshareInfo
		{
			public FileshareInfo(int artifactId, string name, string uncPath)
			{
				ArtifactId = artifactId;
				Name = name;
				UncPath = uncPath;
			}

			public int ArtifactId {  get; }
			public string Name { get; }
			public string UncPath { get; }

			public static FileshareInfo FromJson(ExpandoObject expando)
			{
				IDictionary<string, object> fields = expando;
				return new FileshareInfo( int.Parse(fields["ArtifactID"].ToString()), fields["Name"].ToString(), fields["UNCPath"].ToString());
			}
		}

		private class WorkspaceFilesharesRetriever
		{
			const string GetFileshareServerUri = "/relativity.rest/api/Relativity.Services.Workspace.IWorkspaceModule/Workspace%20Manager%20Service/";
			const string GetFilesharesMethodName = "GetAssociatedFileShareResourceServersAsync";  // this only reads outh client data ( which means secret can be outdated) 
			const string MediaTypeApplicationJson = "application/json";

			private readonly Uri _baseUri;
			public WorkspaceFilesharesRetriever(string relativityInstanceAddress)
			{
				_baseUri = new Uri(new Uri(relativityInstanceAddress), GetFileshareServerUri);
			}


			/// <summary>
			/// retrieve list of fileshares related with specified workspace id 
			/// </summary>
			/// <param name="workspaceID">id of workspace. Use -1 for default workspace</param>
			/// <param name="clientLogin"></param>
			/// <param name="clientPassword"></param>
			/// <returns>
			/// List of fileshare items.
			/// Sample single item JSON:
			///{
			///  "Status": {
			///    "ArtifactID": 0,
			///    "Guids": []
			///},
			///  "UNCPath": "\\\\files.t025.r1.kcura.com\\T025\\Files\\",
			///  "SystemCreatedOn": "0001-01-01T00:00:00",
			///  "SystemLastModifiedOn": "0001-01-01T00:00:00",
			///  "ArtifactID": 1014887,
			///  "Name": "\\\\files.t025.r1.kcura.com\\T025\\Files\\",
			///  "ServerType": {
			///    "ArtifactID": 0
			///  }
			///},
			/// </returns>
			public async Task<IEnumerable<FileshareInfo>> GetWorkspaceFilesharesAsync(int workspaceID, string clientLogin, string clientPassword)
			{
				string base64EncodedBasicCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientLogin + ":" + clientPassword));

				HttpClient _httpClient = new HttpClient();
				_httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");
				_httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedBasicCredentials}");

				Uri endpoint = new Uri(_baseUri, GetFilesharesMethodName);
				HttpContent body = new StringContent($"{{ workspace: {{ ArtifactId:'{workspaceID}'}}}}", Encoding.UTF8, MediaTypeApplicationJson);
				HttpResponseMessage response = await _httpClient.PostAsync(endpoint, body);
				List<ExpandoObject> fileshares = JsonConvert.DeserializeObject<List<ExpandoObject>>(await response.Content.ReadAsStringAsync());
				return fileshares.Select(FileshareInfo.FromJson).ToList();
			}
		}

		private FileshareInfo ChooseFileshare(IEnumerable<FileshareInfo> fileshares)
		{
			FileshareInfo choosenFileshare = null;
			while (choosenFileshare == null)
			{
				Console.WriteLine("  Fileshares: ");
				foreach (var f in fileshares)
				{
					Console.WriteLine($"    - ID: {f.ArtifactId}, Unc Path: {f.UncPath}");
				}
				string fileshareIdStr = _consoleHelper.EnterUntilValid("Choose fileshare to use by typing its artifact id", "[-]*[0-9]+");
				int fileshareId = int.Parse(fileshareIdStr);
				choosenFileshare = fileshares.FirstOrDefault(x => x.ArtifactId == fileshareId);
				if (choosenFileshare == null)
				{
					Console.WriteLine("  Can not find fileshare with provided id.");
				}
			}
			return choosenFileshare;
		}

		private DirectoryPath GetDestinationPath( string fileshareRoot, string finalFolder)
		{
			Console.Write("  Enter relative fileshare destination path (leave empty to keep current path): ");
			string destinationRelativePath = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(destinationRelativePath))
			{
				destinationRelativePath = _consoleHelper.GetOrEnterSetting(SettingNames.FileshareRelativeDestinationPath);
			}
			return new DirectoryPath(Path.Combine(fileshareRoot, destinationRelativePath, finalFolder));
		}
	}
}
