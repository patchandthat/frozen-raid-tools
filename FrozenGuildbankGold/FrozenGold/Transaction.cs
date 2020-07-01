using System;
using System.Collections.Generic;

namespace FrozenGold
{
    public class Transaction : IEquatable<Transaction>
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

        public bool Equals(Transaction other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && Equals(Amount, other.Amount) && PlayerFrom == other.PlayerFrom && PlayerTo == other.PlayerTo && WhenServerTime.Equals(other.WhenServerTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transaction) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Type, Amount, PlayerFrom, PlayerTo, WhenServerTime);
        }
    }
}