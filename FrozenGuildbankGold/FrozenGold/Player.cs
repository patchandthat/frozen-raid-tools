using System;
using System.Collections.Generic;
using System.Linq;

namespace FrozenGold
{
    public class Player : IEquatable<Player>
    {
        private List<Character> _alts;

        public Player(string main, params string[] alts)
        {
            Main = new Character(main);
            _alts = new List<Character>();
            if (alts != null) _alts.AddRange(alts.Select(alt => new Character(alt)));
        }
        
        public Player(Character main, params Character[] alts)
        {
            Main = main;
            _alts = new List<Character>();
            if (alts != null) _alts.AddRange(alts);
        }

        public Character Main { get; private set; }
        public bool IsRetired { get; set; }
        public DateTime JoinedOn { get; set; }

        public IReadOnlyList<Character> Alts
        {
            get { return _alts; }
        }

        public void AddAlt(string name)
        {
            AddAlt(new Character(name));
        }
        
        public void AddAlt(Character alt)
        {
            _alts.Add(alt);
        }

        public bool Owns(string charName)
        {
            return Main.Name == charName ||
                   _alts.Any(a => a.Name == charName);
        }

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Main, other.Main);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Player) obj);
        }

        public override int GetHashCode()
        {
            return (Main != null ? Main.GetHashCode() : 0);
        }
    }
}