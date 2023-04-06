namespace Relativity.Transfer.SDK.Sample.Monitoring
{
    using ByteSizeLib;
    using Relativity.Transfer.SDK.Interfaces.ProgressReporting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StatisticHook
    {
        public static void OnStatisticsReceived(TransferJobStatistics statistics)
        {
            ConsoleHelper.WriteLine(BuildStatisticsMessage(statistics), ConsoleColor.Blue);
        }

        public static void OnSucceededItemReceived(TransferItemState update)
        {
            ConsoleHelper.WriteLine($" {update.Timestamp:T}: Element transferred from {update.Source} to {update.Destination}", ConsoleColor.DarkGreen);
        }

        public static void OnSkippedItemReceived(TransferItemState update)
        {
            ConsoleHelper.WriteLine($" {update.Timestamp:T}: Element skipped. Source: {update.Source} Destination:{update.Destination} Error: {update.Exception?.Message}",
                ConsoleColor.Yellow);
        }

        public static void OnFailedItemReceived(TransferItemState update)
        {
            ConsoleHelper.WriteLine($" {update.Timestamp:T}: Element failed: {update.Source}. Error: {update.Exception?.Message}", ConsoleColor.Red);
        }

        public static void OnProgressStepsReceived(IEnumerable<StepProgress> progressSteps)
        {
            ConsoleHelper.WriteLine(BuildStepProgressMessage(progressSteps), ConsoleColor.Cyan);
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
