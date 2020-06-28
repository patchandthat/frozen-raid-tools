using System;
using System.Linq;

namespace FrozenGold
{
    public class Tariff
    {
        public TariffItem CurrentRate => History.LastOrDefault();
        public TariffItem[] History { get; protected set; }
    }

    public class TariffItem
    {
        public DateTimeOffset BeginsOn { get; set; }
        public TimeSpan RepeatInterval { get; set; }
        public CurrencyAmount Amount { get; set; }
    }
}