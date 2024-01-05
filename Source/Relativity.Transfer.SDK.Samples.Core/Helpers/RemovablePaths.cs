using System;
using System.IO;
using Relativity.Transfer.SDK.Interfaces.Paths;

namespace Relativity.Transfer.SDK.Samples.Core.Helpers;

internal sealed class DisposablePath<TPathBase>(TPathBase path) : IDisposable
	where TPathBase : PathBase
{
	public TPathBase Path { get; } = path;

	public void Dispose()
	{
		try
		{
			Action<string> action = Path is FilePath ? DeleteFile : DeleteDirectory;
			action(Path.ToString());
		}
		catch
		{
			// ignored
		}
	}

	private static void DeleteDirectory(string path)
	{
		if (!Directory.Exists(path)) return;

		Directory.Delete(path, true);
	}

	private static void DeleteFile(string path)
	{
		if (!File.Exists(path)) return;

		File.Delete(path);
	}
}