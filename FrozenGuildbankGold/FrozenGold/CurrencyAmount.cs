using System;

namespace FrozenGold
{
    public class CurrencyAmount : IEquatable<CurrencyAmount>
    {
        public CurrencyAmount(uint gold, uint silver, uint copper)
        {
            Gold = gold;
            Silver = silver;
            Copper = copper;

            if (Copper >= 100)
            {
                uint lower = Copper % 100;
                uint upper = (Copper - lower) / 100;

                Copper = lower;
                Silver += upper;
            }

            if (Silver >= 100)
            {
                uint lower = Silver % 100;
                uint upper = (Silver - lower) / 100;

                Silver = lower;
                Gold += upper;
            }
        }

        public uint Gold { get; }
        public uint Silver { get; }
        public uint Copper { get; }

        public uint TotalCopper => Copper + 100 * Silver + 10000 * Gold;

        public static CurrencyAmount Zero => new CurrencyAmount(0,0,0);

        public static CurrencyAmount FromGold(uint gold)
        {
            return new CurrencyAmount(gold, 0, 0);
        }
        
        public static CurrencyAmount FromSilver(uint silver)
        {
            return new CurrencyAmount(0, silver, 0);
        }
        
        public static CurrencyAmount FromCopper(uint copper)
        {
            return new CurrencyAmount(0, 0, copper);
        }

        public override string ToString()
        {
            return $"{Gold}g {Silver}s {Copper}c";
        }

        public bool Equals(CurrencyAmount other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Gold == other.Gold && Silver == other.Silver && Copper == other.Copper;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CurrencyAmount) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Gold, Silver, Copper);
        }
        
        public static CurrencyAmount operator *(CurrencyAmount amount, int multiplier)
            => FromCopper((uint) (amount.TotalCopper * multiplier));
        
        public static CurrencyAmount operator *(int multiplier, CurrencyAmount amount)
            => FromCopper((uint) (amount.TotalCopper * multiplier));

        public static CurrencyAmount operator +(CurrencyAmount a, CurrencyAmount b)
            => FromCopper(a.TotalCopper + b.TotalCopper);

        public static CurrencyAmount operator -(CurrencyAmount a, CurrencyAmount b)
            => FromCopper(a.TotalCopper - b.TotalCopper);

        public static bool operator ==(CurrencyAmount a, CurrencyAmount b)
            => Equals(a, b);
        
        public static bool operator !=(CurrencyAmount a, CurrencyAmount b)
            => !Equals(a, b);
    }
}