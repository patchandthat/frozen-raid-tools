using System;
using System.Diagnostics.CodeAnalysis;

namespace FrozenGold
{
    public class PlayerReport
    {
        private readonly TariffItem _currentTariff;
        private PlayerReportSummary _summary;
        private CurrencyAmount _amountPaid;
        private CurrencyAmount _amountDueToDate;

        public PlayerReport(Player player, TariffItem currentTariff)
        {
            _currentTariff = currentTariff ?? throw new ArgumentNullException(nameof(currentTariff));
            Player = player ?? throw new ArgumentNullException(nameof(player));
                
            AmountPaid = CurrencyAmount.Zero;
            AmountDueToDate = CurrencyAmount.Zero;
        }

        public Player Player { get; }

        public CurrencyAmount AmountPaid
        {
            get { return _amountPaid; }
            set
            {
                _summary = null;
                _amountPaid = value;
            }
        }

        public CurrencyAmount AmountDueToDate
        {
            get { return _amountDueToDate; }
            set
            {
                _summary = null;
                _amountDueToDate = value;
            }
        }

        public PlayerReportSummary ReportSummary
        {
            get
            {
                if (_summary != null) return _summary;

                if (AmountPaid == AmountDueToDate)
                {
                    _summary = new PlayerReportSummary(
                        PlayerPaymentStatus.PaidInFull,
                        0);
                }
                else if (AmountPaid > AmountDueToDate)
                {
                    var surplus = AmountPaid - AmountDueToDate;
                    int weeksCovered = 0;
                    var amount = _currentTariff.Amount;
                    
                    while (amount <= surplus)
                    {
                        weeksCovered++;
                        amount += _currentTariff.Amount;                        
                    }

                    _summary = new PlayerReportSummary(
                        PlayerPaymentStatus.Ahead,
                        weeksCovered);
                }
                else
                {
                    var deficit = AmountDueToDate - AmountPaid;
                    int weeksBehind = 1;
                    var amount = _currentTariff.Amount;
                    
                    while (amount < deficit)
                    {
                        weeksBehind++;
                        amount += _currentTariff.Amount;                        
                    }

                    _summary = new PlayerReportSummary(
                        PlayerPaymentStatus.Behind,
                        weeksBehind);
                }

                return _summary;
            }
        }
    }
}