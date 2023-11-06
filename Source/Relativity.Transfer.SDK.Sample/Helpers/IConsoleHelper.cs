using System.Threading.Tasks;
using Relativity.Transfer.SDK.Interfaces.Paths;
using Relativity.Transfer.SDK.Sample.Samples;

namespace Relativity.Transfer.SDK.Sample.Helpers
{
    internal interface IConsoleHelper
    {
        void RegisterSample(char key, SampleBase sample);

        bool InitStartupSettings();

        Task RunMenuAsync();

        string GetOrEnterSetting(string settingName, bool printValueIfAlreadySet = true);

        string EnterUntilValid(string askingtext, string validationPattern);

        DirectoryPath GetDestinationDirectoryPath(string transferJobId);

        DirectoryPath EnterSourceDirectoryPathOrTakeDefault();

        FilePath EnterSourceFilePathOrTakeDefault();
    }
}