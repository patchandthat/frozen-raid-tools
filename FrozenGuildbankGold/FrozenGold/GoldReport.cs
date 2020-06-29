using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold
{
    public class GoldReport
    {
        public const string Taxman = "Frozengold";
        public const string Banker = "Frozbank";
        
        private readonly IDataSource _dataSource;

        private readonly Roster _roster;
        private readonly Tariff _tariff;
        private readonly IReadOnlyList<Transaction> _transactions;
        private readonly List<Transaction> _oddTransactions;
        private readonly List<Transaction> _refunds;

        public GoldReport(IDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            _roster = dataSource.GetRoster();
            _tariff = dataSource.GetTariff();
            _transactions = dataSource.GetTransactionHistory();
            LastUpdated = dataSource.GetLastUpdatedDate();
            
            Received = CurrencyAmount.Zero;
            Refunded = CurrencyAmount.Zero;
            SentToBanker = CurrencyAmount.Zero;
            MailboxFees = CurrencyAmount.Zero;
            PlayerReports = Enumerable.Empty<PlayerReport>();
            _oddTransactions = new List<Transaction>();
            _refunds = new List<Transaction>();
        }

        public CurrencyAmount Received { get; private set; }
        public CurrencyAmount SentToBanker { get; private set; }
        public CurrencyAmount MailboxFees { get; private set; }
        public CurrencyAmount Refunded { get; private set; }
        public CurrencyAmount GoldOnHand => Received - (SentToBanker + MailboxFees + Refunded);

        public IEnumerable<Transaction> AllTransactions => _transactions;

        /// <summary>
        /// Should be empty unless gold was received from an unrecognised character
        /// The roster probably needs updating for those foreign types with accents in their names.
        /// </summary>
        public IEnumerable<Transaction> OddTransactions => _oddTransactions;
        public IEnumerable<Transaction> Refunds => _refunds;
        public IEnumerable<PlayerReport> PlayerReports { get; private set; }
        public DateTimeOffset LastUpdated { get; }

        public void BuildReport()
        {
            Reset();

            var playerReports = new Dictionary<Player, PlayerReport>();

            CalculateDues(playerReports);
            ProcessTransactions(playerReports);

            PlayerReports = playerReports
                .Select(kvp => kvp.Value)
                .ToList();
        }

        private void Reset()
        {
            _oddTransactions.Clear();
            _refunds.Clear();
            Received = CurrencyAmount.Zero;
            Refunded = CurrencyAmount.Zero;
            SentToBanker = CurrencyAmount.Zero;
            MailboxFees = CurrencyAmount.Zero;
        }

        private void CalculateDues(Dictionary<Player, PlayerReport> playerReports)
        {
            foreach (Player player in _roster.Players)
            {
                playerReports[player] = new PlayerReport(player, _tariff.CurrentRate);

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
                    
                    if (player.IsRetired && player.LeftOn < intervalEndDate)
                        break;
                    
                    if (player.JoinedOn < intervalEndDate)
                        playerReports[player].AmountDueToDate += currentRate.Amount;

                    currentDate = intervalEndDate;
                }
            }
        }
        
        private void ProcessTransactions(Dictionary<Player, PlayerReport> playerReports)
        {
            foreach (var tx in _transactions)
            {
                if (tx.PlayerTo.Equals(Taxman, StringComparison.CurrentCultureIgnoreCase))
                {
                    Received += tx.Amount;
                    
                    var player = _roster.Find(tx.PlayerFrom);
                    if (player == null)
                    {
                        _oddTransactions.Add(tx);
                        continue;
                    }

                    var report = playerReports[player];
                    report.AmountPaid += tx.Amount;
                }
                else if (tx.PlayerTo.Equals(Banker, StringComparison.CurrentCultureIgnoreCase) && 
                         tx.PlayerFrom.Equals(Taxman, StringComparison.CurrentCultureIgnoreCase))
                {
                    SentToBanker += tx.Amount;
                    MailboxFees += CurrencyAmount.FromCopper(30);
                }
                else if (tx.PlayerFrom.Equals(Taxman, StringComparison.CurrentCultureIgnoreCase))
                {
                    var player = _roster.Find(tx.PlayerTo);
                    if (player == null)
                    {
                        _oddTransactions.Add(tx);
                        continue;
                    }

                    // Account for refunds where the player left the raid team
                    if (player.IsRetired)
                    {
                        var report = playerReports[player];
                        report.AmountPaid -= tx.Amount;
                        Refunded += tx.Amount;
                        _refunds.Add(tx);
                    }
                }
                else
                {
                    // Shouldn't really have any txns that don't fall into the two cases above...
                    _oddTransactions.Add(tx);
                }
            }
        }
    }
}