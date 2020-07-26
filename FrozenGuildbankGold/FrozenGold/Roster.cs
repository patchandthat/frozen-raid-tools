using System.Collections.Generic;
using System.Linq;

namespace FrozenGold
{
    public class Roster
    {
        private List<Player> _players = new List<Player>();

        public IEnumerable<Player> Players
        {
            get { return _players; }
            set { _players = value.ToList(); }
        }
        
        public virtual Roster Add(Player player)
        {
            if (_players.Contains(player)) 
                throw new DuplicateException($"Player '{player.Main.Name}' is already in the roster");
            
            _players.Add(player);
            return this;
        }

        public virtual Player Find(string charName)
        {
            foreach (var player in _players)
            {
                if (player.Owns(charName))
                    return player;
            }

            return null;
        }
    }
}