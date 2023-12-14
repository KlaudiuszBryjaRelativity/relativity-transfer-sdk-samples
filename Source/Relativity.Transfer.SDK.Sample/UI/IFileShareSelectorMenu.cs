using System.Threading;
using Relativity.Transfer.SDK.Sample.DTOs;

namespace Relativity.Transfer.SDK.Sample.UI;

internal interface IFileShareSelectorMenu
{
	FileShareInfo SelectFileShare(FileShareInfo[] fileShareInfos, CancellationToken token);
}