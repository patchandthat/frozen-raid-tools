using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold.Web.Services
{
    public class DataSource : IDataSource
    {
        private TransactionHistory _txns;
        private readonly Roster _roster;

        public DateTimeOffset NowServerTime => DateTimeOffset.UtcNow;

        public DataSource(TransactionHistory txns, PlayersDto players)
        {
            _txns = txns;
            _roster = new RosterBuilder(players).Build();
        }

        public DateTimeOffset GetLastUpdatedDate()
        {
            return _txns.LastUpdated;
        }

        public Roster GetRoster()
        {
            return _roster;
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
