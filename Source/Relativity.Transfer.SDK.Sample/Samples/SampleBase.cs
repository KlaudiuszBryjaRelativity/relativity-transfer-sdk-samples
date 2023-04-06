using System.Threading.Tasks;

namespace Relativity.Transfer.SDK.Sample.Samples
{
	public abstract class SampleBase
	{
		protected ConsoleHelper _consoleHelper; 

		public SampleBase( ConsoleHelper consoleHelper)
		{
			_consoleHelper = consoleHelper;
		}

		public virtual string Descritpion { get {  return GetType().Name; } }
		public abstract Task ExecuteAsync();
	}
}
