using System;

namespace FrozenGold.Api
{
    public class TransactionHistory
    {
        public DateTimeOffset LastUpdated { get; set; }
        public Transaction[] Transactions { get; set; }
    }
}
