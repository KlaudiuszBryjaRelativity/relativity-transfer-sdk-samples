namespace Relativity.Transfer.SDK.Sample.Monitoring
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ByteSizeLib;
	using Interfaces.ProgressReporting;
	using Core.ProgressReporting;

	internal static class ConsoleStatisticHook
	{

		public static ITransferProgressHandler GetProgressHandler()
		{
			// Transfer progress is optional - you can subscribe to all, some, or no updates. Bear in mind though that this is the only way you can obtain information
			// about eventual fails or skips on individual items and that's why encourage the clients to take an advantage of it (subscribe and log it).
			// TransferSDK doesn't store any information about failed file paths due to privacy concerns.
			return TransferProgressHandlerBuilder.Instance
				.OnStatistics(OnStatisticsReceived) // Updates about overall status (% progress, transfer rates, partial statistics)
				.OnSucceededItem(OnSucceededItemReceived) // Updates on each transferred item
				.OnFailedItem(OnFailedItemReceived) // Updates on each failed item (and reason for it)
				.OnSkippedItem(OnSkippedItemReceived) // Updates on each skipped item (and reason for it)
				.OnProgressSteps(OnProgressStepsReceived) // Updates on each job's progress steps. Use it to track overall percentage progress and state.
				.Create();
		}

		private static void OnStatisticsReceived(TransferJobStatistics statistics)
		{
			WriteLine(BuildStatisticsMessage(statistics), ConsoleColor.Blue);
		}

		private static void OnSucceededItemReceived(TransferItemState update)
		{
			WriteLine($" {update.Timestamp:T}: Element transferred from {update.Source} to {update.Destination}", ConsoleColor.DarkGreen);
		}

		private static void OnSkippedItemReceived(TransferItemState update)
		{
			WriteLine($" {update.Timestamp:T}: Element skipped. Source: {update.Source} Destination:{update.Destination} Error: {update.Exception?.Message}", ConsoleColor.Yellow);
		}

		private static void OnFailedItemReceived(TransferItemState update)
		{
			WriteLine($" {update.Timestamp:T}: Element failed: {update.Source}. Error: {update.Exception?.Message}", ConsoleColor.Red);
		}

		private static void OnProgressStepsReceived(IEnumerable<StepProgress> progressSteps)
		{
			WriteLine(BuildStepProgressMessage(progressSteps), ConsoleColor.Cyan);
		}

		private static void WriteLine(string line, ConsoleColor color)
		{
			ConsoleColor defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(line);
			Console.ForegroundColor = defaultColor;
		}

		private static string BuildStatisticsMessage(TransferJobStatistics statistics)
		{
			const string notKnown = "-";
			string totalMegaBytes = statistics.TotalBytes.HasValue ? $"{ByteSize.FromBytes((double)statistics.TotalBytes).MegaBytes:F}" : notKnown;
			var bytesStatistic = $"{ByteSize.FromBytes(statistics.CurrentBytesTransferred).MegaBytes:F}/{totalMegaBytes}";

			string totalItems = statistics.TotalItems.HasValue ? statistics.TotalItems.ToString() : notKnown;
			var transferredItems = $"{statistics.CurrentItemsTransferred}/{totalItems}";

			string estimatedTime = statistics.EstimatedTime.HasValue ? statistics.EstimatedTime.Value.ToString(@"h\:mm\:ss") : notKnown;

			return
				$"{statistics.Timestamp:T}: {bytesStatistic} MB, Items: {transferredItems}, Skipped: {statistics.CurrentItemsSkipped}, Failed: {statistics.CurrentItemsFailed}, ETA: {estimatedTime}";
		}

		private static string BuildStepProgressMessage(IEnumerable<StepProgress> progressSteps)
		{
			return string.Join(Environment.NewLine,
				progressSteps.Select(stepProgress => $"  Step name: {stepProgress.Name}, {stepProgress.PercentageProgress:F}%, State: {stepProgress.State}"));
		}
	}
}
