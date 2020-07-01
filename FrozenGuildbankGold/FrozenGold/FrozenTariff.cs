using System;

namespace FrozenGold
{
    public class FrozenTariff : Tariff
    {
        public FrozenTariff()
        {
            History = new[]
            {
                new TariffItem
                {
                    BeginsOn = new DateTimeOffset(2020, 06, 24, 00, 00, 00, TimeSpan.FromHours(1)),
                    RepeatInterval = TimeSpan.FromDays(7),
                    Amount = new CurrencyAmount(40, 0, 0)
                }, 
            };
        }
    }
}