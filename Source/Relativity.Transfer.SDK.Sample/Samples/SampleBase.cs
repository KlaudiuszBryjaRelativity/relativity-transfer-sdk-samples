namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System.Threading.Tasks;
	using Helpers;

	internal abstract class SampleBase
	{
		protected IConsoleHelper _consoleHelper;

		protected SampleBase(IConsoleHelper consoleHelper)
		{
			_consoleHelper = consoleHelper;
		}

		public virtual string Description => GetType().Name;
		public abstract Task ExecuteAsync();
	}
}
