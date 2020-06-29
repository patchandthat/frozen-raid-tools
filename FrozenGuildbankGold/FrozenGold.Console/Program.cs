using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace FrozenGold.Console
{
    class ConsoleDataSource : IDataSource
    {
        const string reportfile = "./Accounting_Razorgore_income.csv";
        
        public Roster GetRoster()
        {
            return new FrozenRoster();
        }

        public Tariff GetTariff()
        {
            return new FrozenTariff();
        }

        public IReadOnlyList<Transaction> GetTransactionHistory()
        {
            using (var reader = new StreamReader(reportfile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv
                    .GetRecords<AccountingCsvRow>()
                    .Select(row => row.ToTransaction())
                    .Where(txn =>
                        txn.PlayerFrom == "Frozengold" || txn.PlayerTo == "Frozengold")
                    .ToList();

                return rows;
            }
        }

        public DateTimeOffset GetLastUpdatedDate()
        {
            FileInfo fi = new FileInfo(reportfile);

            return new DateTimeOffset(fi.CreationTimeUtc);
        }

        public DateTimeOffset NowServerTime => DateTimeOffset.UtcNow;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ds = new ConsoleDataSource();
            
            var report = new GoldReport(ds);
            report.BuildReport();

            Print(report);
        }

        private static void Print(GoldReport report)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"===== Frozen Gold Report as of {report.LastUpdated:yyyy MMMM dd} =====");
            
            System.Console.WriteLine("");
            System.Console.WriteLine($"Gold collected: {report.Received}");
            System.Console.WriteLine($"Gold sent:      {report.SentToBanker}"); // txns out missing?
            System.Console.WriteLine($"Gold on hand:   {report.GoldOnHand}");
            
            System.Console.WriteLine("");
            int oddTxnCount = report.OddTransactions.Count();
            System.Console.WriteLine($"=== Odd looking txns: {oddTxnCount} ===");
            if (oddTxnCount > 0)
            {
                foreach (var txn in report.OddTransactions)
                {
                    System.Console.WriteLine($"   {txn.PlayerFrom} sent {txn.PlayerTo} {txn.Amount}");
                }
            }

            System.Console.WriteLine("");

            System.Console.WriteLine("=== By Player ===");
            var ahead = new List<PlayerReport>();
            var behind = new List<PlayerReport>();
            var correct = new List<PlayerReport>();
            foreach (var playerReport in report.PlayerReports.Where(pr => !pr.Player.IsRetired))
            {
                var summary = playerReport.ReportSummary;
                switch (summary.Status)
                {
                    case PlayerPaymentStatus.Behind:
                        behind.Add(playerReport);
                        break;
                    case PlayerPaymentStatus.PaidInFull:
                        correct.Add(playerReport);
                        break;
                    case PlayerPaymentStatus.Ahead:
                        ahead.Add(playerReport);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            System.Console.WriteLine();
            System.Console.WriteLine($"== Behind");
            foreach (PlayerReport playerReport in behind)
            {
                var summary = playerReport.ReportSummary;
                System.Console.WriteLine($"{playerReport.Player} is {Print(summary.Status)} by {summary.WeeksDifference} weeks");
            }
            
            System.Console.WriteLine();
            System.Console.WriteLine($"== Ahead");
            foreach (PlayerReport playerReport in ahead)
            {
                var summary = playerReport.ReportSummary;
                System.Console.WriteLine($"{playerReport.Player} is {Print(summary.Status)} by {summary.WeeksDifference} weeks");
            }
            
            System.Console.WriteLine();
            System.Console.WriteLine($"== Paid");
            foreach (PlayerReport playerReport in correct)
            {
                System.Console.WriteLine($"{playerReport.Player} is up to date");
            }

            System.Console.WriteLine("");
            System.Console.WriteLine("=== Complete transaction log ===");
            foreach (var txn in report.AllTransactions.OrderBy(tx => tx.WhenServerTime))
            {
                System.Console.WriteLine($"   {txn.PlayerFrom} sent {txn.PlayerTo} {txn.Amount} on {txn.WhenServerTime:f}");
            }
        }

        private static string Print(PlayerPaymentStatus summaryStatus)
        {
            switch (summaryStatus)
            {
                case PlayerPaymentStatus.Behind:
                    return "behind";
                case PlayerPaymentStatus.PaidInFull:
                    return "paid";
                case PlayerPaymentStatus.Ahead:
                   return "ahead";
                default:
                    throw new ArgumentOutOfRangeException(nameof(summaryStatus), summaryStatus, null);
            }
        }
    }
}