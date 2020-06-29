using System;

namespace FrozenGold.Web.Services
{
    public class TransactionHistory
    {
        public DateTimeOffset LastUpdated { get; set; }
        public TransactionDto[] Transactions { get; set; }
    }
}
