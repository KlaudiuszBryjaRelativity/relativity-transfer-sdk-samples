using System.Threading;
using Relativity.Transfer.SDK.Samples.Core.DTOs;

namespace Relativity.Transfer.SDK.Samples.Core.UI;

internal interface IFileShareSelectorMenu
{
	FileShareInfo SelectFileShare(FileShareInfo[] fileShareInfos, CancellationToken token);
}