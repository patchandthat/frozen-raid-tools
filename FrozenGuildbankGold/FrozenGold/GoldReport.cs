using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold
{
    public class GoldReport
    {
        public const string Taxman = "FrozenGold";
        public const string Banker = "Frozbank";

        private readonly DateTimeOffset _lastUpdated;
        private readonly Roster _roster;
        private readonly Tariff _tariff;
        private readonly IReadOnlyList<Transaction> _transactions;
        private List<Transaction> _badTransactions;

        public GoldReport(IDataSource dataSource)
        {
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            
            _roster = dataSource.GetRoster();
            _tariff = dataSource.GetTariff();
            _transactions = dataSource.GetTransactionHistory();
            _lastUpdated = dataSource.GetLastUpdatedDate();
        }
        
        public CurrencyAmount Received { get; private set; }
        public CurrencyAmount SentToFrozbank { get; private set; }
        public CurrencyAmount GoldOnHand => Received - SentToFrozbank;

        public IEnumerable<Transaction> AllTransactions => _transactions;

        /// <summary>
        /// Should be empty unless gold was received from an unrecognised character
        /// The roster probably needs updating for those foreign types with accents in their names.
        /// </summary>
        public IEnumerable<Transaction> UnprocessedTransactions => _badTransactions;
        
        public IEnumerable<PlayerReport> PlayerReports { get; private set; }
        
        public class PlayerReport
        {
            public PlayerReport(Player player)
            {
                Player = player;
            }

            public Player Player { get; private set; }
            public CurrencyAmount AmountPaid { get; set; }
            public CurrencyAmount AmountDueToDate { get; set; }
            public PaymentStatus Status { get; }

            /// <summary>
            /// If a player is behind or ahead on payments, this is how much gold in credit/owed
            /// </summary>
            public CurrencyAmount StatusAmount { get; }
        
            /// <summary>
            /// If a player is behind or ahead on payments, the date they are behind since/paid until
            /// </summary>
            public DateTimeOffset? StatusDate { get; }
        }
        
        public void GenerateReport()
        {
            _badTransactions = new List<Transaction>();
            Received = CurrencyAmount.Zero;
            SentToFrozbank = CurrencyAmount.Zero;
            
            var playerReports = new Dictionary<Player, PlayerReport>();

            CalculateDues(playerReports);
            ProcessTransactions(playerReports);

            PlayerReports = playerReports
                .Select(kvp => kvp.Value)
                .ToList();
        }

        private void CalculateDues(Dictionary<Player, PlayerReport> playerReports)
        {
            
        }

        private void ProcessTransactions(Dictionary<Player, PlayerReport> playerReports)
        {
            foreach (Transaction tx in _transactions)
            {
                if (tx.PlayerTo == Taxman)
                {
                    Received += tx.Amount;

                    var player = _roster.Find(tx.PlayerFrom);
                    if (player == null)
                    {
                        _badTransactions.Add(tx);
                        continue;
                    }

                    if (!playerReports.TryGetValue(player, out PlayerReport report))
                    {
                        report = new PlayerReport(player);
                        playerReports[player] = report;
                    }

                    report.AmountPaid += tx.Amount;
                }
                else if (tx.PlayerFrom == Taxman && tx.PlayerTo == Banker)
                {
                    SentToFrozbank += tx.Amount;
                }
                else
                {
                    _badTransactions.Add(tx);
                }
            }
        }
    }

    public enum PaymentStatus
    {
        Deficit,
        Paid,
        Credit
    }
}