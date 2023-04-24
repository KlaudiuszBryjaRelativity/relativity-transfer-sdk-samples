namespace Relativity.Transfer.SDK.Sample.Samples
{
	using System.Threading.Tasks;
	using Helpers;

	internal abstract class SampleBase
	{
		protected ConsoleHelper _consoleHelper;

		protected SampleBase( ConsoleHelper consoleHelper)
		{
			_consoleHelper = consoleHelper;
		}

		public virtual string Description => GetType().Name;
		public abstract Task ExecuteAsync();
	}
}
