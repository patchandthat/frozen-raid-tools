using System;

namespace FrozenGold
{
    public class Transaction
    {
        public Transaction(
            TransactionType type, 
            CurrencyAmount amount, 
            string playerFrom, 
            string playerTo, 
            DateTimeOffset whenServerTime)
        {
            Type = type;
            Amount = amount;
            PlayerFrom = playerFrom;
            PlayerTo = playerTo;
            WhenServerTime = whenServerTime;
        }

        public TransactionType Type { get; }
        public CurrencyAmount Amount { get; }
        public string PlayerFrom { get; }
        public string PlayerTo { get; }
        public DateTimeOffset WhenServerTime { get; }
    }
}