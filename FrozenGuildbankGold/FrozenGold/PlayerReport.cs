using System;

namespace FrozenGold
{
    public class PlayerReport
    {
        private readonly TariffItem _currentTariff;

        public PlayerReport(Player player, TariffItem currentTariff)
        {
            _currentTariff = currentTariff ?? throw new ArgumentNullException(nameof(currentTariff));
            Player = player ?? throw new ArgumentNullException(nameof(player));
                
            AmountPaid = CurrencyAmount.Zero;
            AmountDueToDate = CurrencyAmount.Zero;
        }

        public Player Player { get; }
        public CurrencyAmount AmountPaid { get; set; }
        public CurrencyAmount AmountDueToDate { get; set; }

        public PlayerReportSummary ReportSummary
        {
            get
            {
                if (AmountPaid == AmountDueToDate)
                    return new PlayerReportSummary(
                        PlayerPaymentStatus.PaidInFull, 0);

                if (AmountPaid > AmountDueToDate)
                {
                    var surplus = AmountPaid - AmountDueToDate;
                    int weeksCovered = 0;
                    var amount = _currentTariff.Amount;
                    
                    while (amount <= surplus)
                    {
                        weeksCovered++;
                        amount += _currentTariff.Amount;                        
                    }
                    
                    return new PlayerReportSummary(
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

                    return new PlayerReportSummary(
                        PlayerPaymentStatus.Behind,
                        weeksBehind);
                }
            }
        }
    }
}