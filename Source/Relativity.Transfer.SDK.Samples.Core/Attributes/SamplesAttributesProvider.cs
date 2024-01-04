using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Relativity.Transfer.SDK.Samples.Core.Attributes;

internal static class SamplesAttributesProvider
{
	public static SampleAttribute[] GetSamplesAttributes()
	{
		EnsureAllSamplesAssembliesAreLoaded();

		return AppDomain.CurrentDomain
			.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.GetCustomAttribute(typeof(SampleAttribute), false) != null)
			.Select(type => (SampleAttribute)type.GetCustomAttribute(typeof(SampleAttribute), false))
			.Concat(new[] { SampleAttribute.ExitOptionAttribute })
			.Distinct()
			.OrderBy(attr => attr.Order)
			.ToArray();
	}

	private static void EnsureAllSamplesAssembliesAreLoaded()
	{
		var directoryName = Path.GetDirectoryName(typeof(SampleAttribute).Assembly.Location);
		if (string.IsNullOrWhiteSpace(directoryName))
			throw new InvalidOperationException("Cannot get directory path of the Core assembly.");

		//foreach (var assemblyPath in Directory.GetFiles(directoryName, "Relativity.Transfer.SDK.Samples.*.dll"))
		foreach (var assemblyPath in Directory.GetFiles(directoryName, "*.dll"))
			try
			{
				Assembly.LoadFrom(assemblyPath);
			}
			catch
			{
				//ignore
			}
	}
}