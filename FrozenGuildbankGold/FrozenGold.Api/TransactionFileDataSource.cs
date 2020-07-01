using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace FrozenGold.Api
{
    class TransactionFileDataSource : IDataSource
    {
        const string income = "Accounting_Razorgore_income.csv";
        const string expenses = "Accounting_Razorgore_expenses.csv";
        private readonly IWebHostEnvironment _environment;

        public TransactionFileDataSource(IWebHostEnvironment env)
        {
            _environment = env ?? throw new ArgumentNullException(nameof(env));
        }

        public Roster GetRoster()
        {
            return null;
        }

        public Tariff GetTariff()
        {
            return null;
        }

        public IReadOnlyList<Transaction> GetTransactionHistory()
        {
            var fileProvider = _environment.ContentRootFileProvider;
            var contents = fileProvider.GetDirectoryContents("Data");

            var incomeFile = contents.Single(fi => fi.Name == income);
            var expensefile = contents.Single(fi => fi.Name == expenses);

            using (var incomeReader = new StreamReader(incomeFile.PhysicalPath))
            using (var expenseReader = new StreamReader(expensefile.PhysicalPath))
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
            var fileProvider = _environment.ContentRootFileProvider;
            var contents = fileProvider.GetDirectoryContents("Data");

            var incomeFile = contents.Single(fi => fi.Name == income);

            return incomeFile.LastModified;
        }

        public DateTimeOffset NowServerTime => DateTimeOffset.UtcNow;
    }
}
