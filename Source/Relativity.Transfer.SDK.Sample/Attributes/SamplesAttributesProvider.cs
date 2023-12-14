using System;
using System.Linq;
using System.Reflection;

namespace Relativity.Transfer.SDK.Sample.Attributes;

internal static class SamplesAttributesProvider
{
	public static SampleAttribute[] GetSamplesAttributes()
	{
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
}