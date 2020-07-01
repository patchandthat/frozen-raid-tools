using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold.Web.Services
{
    public class DataSource : IDataSource
    {
        private TransactionHistory _txns;

        public DateTimeOffset NowServerTime => DateTimeOffset.UtcNow;

        public DataSource(TransactionHistory txns)
        {
            _txns = txns;
        }

        public DateTimeOffset GetLastUpdatedDate()
        {
            return _txns.LastUpdated;
        }

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
            return _txns.Transactions
                .Select(tx =>
                    new Transaction(
                        tx.Type,
                        CurrencyAmount.FromCopper(tx.CopperAmount),
                        tx.PlayerFrom,
                        tx.PlayerTo,
                        tx.WhenServerTime
                )).ToList();
        }
    }
}
