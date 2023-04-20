namespace Relativity.Transfer.SDK.Sample.Monitoring
{
	using ByteSizeLib;
	using System;
	using System.Linq;
	using System.Text;
	using Interfaces.ProgressReporting;

	internal class TransferJobSummary
	{
		public TransferJobSummary(TransferJobResult result)
		{
			Result = result;
		}

		private TransferJobResult Result { get; }

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendLine($"  - JobId: {Result.CorrelationId}");
			sb.AppendLine($"  - Total Bytes: {Result.TotalBytes} ({ByteSize.FromBytes(Result.TotalBytes)})");
			sb.AppendLine($"  - Total Files Transferred: {Result.TotalFilesTransferred}");
			sb.AppendLine($"  - Total Empty Directories Transferred: {Result.TotalEmptyDirectoriesTransferred}");
			sb.AppendLine($"  - Total Files Skipped: {Result.TotalFilesSkipped}");
			sb.AppendLine($"  - Total Files Failed: {Result.TotalFilesFailed}");
			sb.AppendLine($"  - Total Empty Directories Failed: {Result.TotalEmptyDirectoriesFailed}");
			sb.AppendLine($"  - Elapsed: {Result.Elapsed:hh\\:mm\\:ss} s ({Math.Floor(Result.Elapsed.TotalSeconds)} s)");
			sb.AppendLine($"  - Status: {Result.State.Status}");
			AddProgressStepsDescription(sb);

			return sb.ToString();
		}

		private void AddProgressStepsDescription(StringBuilder sb)
		{
			if (!Result.ProgressSteps.Any())
			{
				return;
			}

			sb.AppendLine("  - Job's steps:");
			foreach (StepProgress stepProgress in Result.ProgressSteps)
			{
				sb.AppendLine($"      Step '{stepProgress.Name}': {stepProgress.PercentageProgress:F}%, {stepProgress.State}");
			}
		}
	}
}
