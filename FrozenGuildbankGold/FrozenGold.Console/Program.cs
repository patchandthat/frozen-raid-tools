using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace FrozenGold.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string reportfile = ".\\Accounting_Razorgore_income.csv";
            
            using (var reader = new StreamReader(reportfile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rows = csv
                    .GetRecords<AccountingCsvRow>()
                    .Select(row => row.ToTransaction())
                    .Where(txn =>
                        txn.Type == TransactionType.MoneyTransfer &&
                        (txn.PlayerFrom == "Frozengold" || txn.PlayerTo == "Frozengold"))
                    .ToList();

                Print(rows);
            }
        }

        private static void Print(List<Transaction> rows)
        {
            System.Console.WriteLine($"FrozenGold Transactions:");

            if (rows.Count == 0)
            {
                System.Console.WriteLine("... No transactions.");
                return;
            }
            
            foreach (var tx in rows)
            {
                System.Console.WriteLine($"\t{tx.PlayerFrom} sent {tx.PlayerTo} {tx.Amount} on {tx.WhenServerTime:G}ST");
            }
            
            System.Console.WriteLine("End of transactions.");
        }
    }
}