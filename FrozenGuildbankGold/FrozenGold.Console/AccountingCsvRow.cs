using System;
using CsvHelper.Configuration.Attributes;

namespace FrozenGold.Console
{
    public class AccountingCsvRow
    {
        [Name("type")]
        public string Type { get; set; }
        
        [Name("amount")]
        public uint Amount { get; set; }
        
        [Name("otherPlayer")]
        public string OtherPlayer { get; set; }
        
        [Name("player")]
        public string Player { get; set; }
        
        [Name("time")]
        public long Time { get; set; }

        public Transaction ToIncomeTransaction()
        {
            return new Transaction(
                Type == "Money Transfer" ? TransactionType.MoneyTransfer : TransactionType.Other,
                CurrencyAmount.FromCopper(Amount), 
                OtherPlayer,
                Player,
                DateTimeOffset.FromUnixTimeSeconds(Time));
        }
        
        public Transaction ToExpenseTransaction()
        {
            return new Transaction(
                Type == "Money Transfer" ? TransactionType.MoneyTransfer : TransactionType.Other,
                CurrencyAmount.FromCopper(Amount),
                Player,
                OtherPlayer,
                DateTimeOffset.FromUnixTimeSeconds(Time));
        }
    }
}