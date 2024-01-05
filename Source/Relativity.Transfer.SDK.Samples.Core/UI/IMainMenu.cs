using System;
using System.Threading;
using System.Threading.Tasks;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal interface IMainMenu
{
	Task ShowAsync(CancellationToken token, Func<Task> postScreenAction = null);
}