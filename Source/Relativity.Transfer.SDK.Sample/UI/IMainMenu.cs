using System.Threading.Tasks;
using System.Threading;
using System;

namespace Relativity.Transfer.SDK.Sample.UI;

internal interface IMainMenu
{
	Task ShowAsync(CancellationToken token, Func<Task> postScreenAction = null);
}