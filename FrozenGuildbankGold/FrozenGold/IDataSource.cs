using System;
using System.Collections.Generic;

namespace FrozenGold
{
    public interface IDataSource
    {
        public Roster GetRoster();
        public Tariff GetTariff();
        public IReadOnlyList<Transaction> GetTransactionHistory();
        public DateTimeOffset GetLastUpdatedDate();
        
        DateTimeOffset NowServerTime { get; }
    }
}