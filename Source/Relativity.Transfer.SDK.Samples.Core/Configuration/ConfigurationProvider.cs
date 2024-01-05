using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Relativity.Transfer.SDK.Samples.Core.Configuration;

internal static class ConfigurationProvider
{
	private const string ConfigurationFileName = "appsettings.json";

	public static Configuration GetConfiguration()
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
			.AddJsonFile(ConfigurationFileName)
			.Build();

		return configuration.Get<Configuration>();
	}
}