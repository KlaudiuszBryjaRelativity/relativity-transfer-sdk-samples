using System;
using System.IO;
using Relativity.Transfer.SDK.Interfaces.Paths;

namespace Relativity.Transfer.SDK.Sample.Helpers;

internal sealed class DisposablePath<TPathBase> : IDisposable
	where TPathBase : PathBase
{
	public DisposablePath(TPathBase path)
	{
		Path = path;
	}

	public TPathBase Path { get; }

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