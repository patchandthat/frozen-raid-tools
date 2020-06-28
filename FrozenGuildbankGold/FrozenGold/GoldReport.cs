using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold
{
    public class GoldReport
    {
        private readonly IDataSource _dataSource;
        public const string Taxman = "FrozenGold";
        public const string Banker = "Frozbank";

        private readonly Roster _roster;
        private readonly Tariff _tariff;
        private readonly IReadOnlyList<Transaction> _transactions;
        private List<Transaction> _badTransactions;

        public GoldReport(IDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _roster = dataSource.GetRoster();
            _tariff = dataSource.GetTariff();
            _transactions = dataSource.GetTransactionHistory();
            LastUpdated = dataSource.GetLastUpdatedDate();
            
            Received = CurrencyAmount.Zero;
            SentToBanker = CurrencyAmount.Zero;
            PlayerReports = Enumerable.Empty<PlayerReport>();
        }
        
        public CurrencyAmount Received { get; private set; }
        public CurrencyAmount SentToBanker { get; private set; }
        public CurrencyAmount GoldOnHand => Received - SentToBanker;

        public IEnumerable<Transaction> AllTransactions => _transactions;

        /// <summary>
        /// Should be empty unless gold was received from an unrecognised character
        /// The roster probably needs updating for those foreign types with accents in their names.
        /// </summary>
        public IEnumerable<Transaction> UnprocessedTransactions => _badTransactions;
        
        public IEnumerable<PlayerReport> PlayerReports { get; private set; }
        public DateTimeOffset LastUpdated { get; }

        public class PlayerReport
        {
            public PlayerReport(Player player)
            {
                Player = player;
                
                AmountPaid = CurrencyAmount.Zero;
                AmountDueToDate = CurrencyAmount.Zero;
            }

            public Player Player { get; private set; }
            public CurrencyAmount AmountPaid { get; set; }
            public CurrencyAmount AmountDueToDate { get; set; }
            
            // Todo: Get Summary
        }
        
        public void BuildReport()
        {
            _badTransactions = new List<Transaction>();
            Received = CurrencyAmount.Zero;
            SentToBanker = CurrencyAmount.Zero;
            
            var playerReports = new Dictionary<Player, PlayerReport>();

            CalculateDues(playerReports);
            // ProcessTransactions(playerReports);

            PlayerReports = playerReports
                .Select(kvp => kvp.Value)
                .ToList();
        }

        private void CalculateDues(Dictionary<Player, PlayerReport> playerReports)
        {
            foreach (Player player in _roster.Players)
            {
                playerReports[player] = new PlayerReport(player);

                int tariffIndex = 0;
                TariffItem currentRate = _tariff.History[tariffIndex];
                var currentDate = currentRate.BeginsOn;

                while (currentDate < _dataSource.NowServerTime)
                {
                    if (currentRate != _tariff.CurrentRate)
                    {
                        var nextRate = _tariff.History[tariffIndex + 1];
                        if (nextRate.BeginsOn <= currentDate)
                        {
                            currentRate = nextRate;
                            tariffIndex++;
                        }
                    }
                    
                    var intervalEndDate = currentDate + currentRate.RepeatInterval;
                    
                    if (player.IsRetired && player.RetiredOn < intervalEndDate)
                        break;
                    
                    if (player.JoinedOn < intervalEndDate)
                        playerReports[player].AmountDueToDate += currentRate.Amount;

                    currentDate = intervalEndDate;
                }
            }
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
                    SentToBanker += tx.Amount;
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