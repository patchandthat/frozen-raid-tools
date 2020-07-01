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
        const string income = "./Accounting_Razorgore_income.csv";
        const string expenses = "./Accounting_Razorgore_expenses.csv";
        
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
            using (var incomeReader = new StreamReader(income))
            using (var expenseReader = new StreamReader(expenses))
            using (var incomeCsv = new CsvReader(incomeReader, CultureInfo.InvariantCulture))
            using (var expenseCsv = new CsvReader(expenseReader, CultureInfo.InvariantCulture))
            {
                var expenseRows = expenseCsv
                    .GetRecords<AccountingCsvRow>()
                    .Select(row => row.ToExpenseTransaction())
                    .Where(row => // These rows are already in the income report for FrozenGold, exclude them from the expense report of Neffer 
                        !(row.PlayerFrom.Equals("Neffer", StringComparison.CurrentCultureIgnoreCase) &&
                          row.PlayerTo.Equals("Frozengold", StringComparison.OrdinalIgnoreCase)));
                
                var rows = incomeCsv
                    .GetRecords<AccountingCsvRow>()
                    .Select(row => row.ToIncomeTransaction())
                    .Concat(expenseRows)
                    .Where(txn =>
                        txn.Type == TransactionType.MoneyTransfer &&
                        (txn.PlayerFrom.Equals("Frozengold", StringComparison.CurrentCultureIgnoreCase) || 
                         txn.PlayerTo.Equals("Frozengold", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                return rows;
            }
        }

        public DateTimeOffset GetLastUpdatedDate()
        {
            FileInfo fi = new FileInfo(income);

            return new DateTimeOffset(fi.CreationTimeUtc);
        }

        public DateTimeOffset NowServerTime => DateTimeOffset.UtcNow;
    }
}