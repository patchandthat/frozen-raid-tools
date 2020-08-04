using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrozenGold.Web.Services
{
    public partial class PlayersDto
    {
       public Player[] Players { get; set; }
    }

    public partial class Player
    {
        public Character Main { get; set; }

        public DateTimeOffset? LeftOn { get; set; }

        public DateTimeOffset? JoinedOn { get; set; }

        public Character[] Alts { get; set; }
    }

    public partial class Character
    {
        public string Name { get; set; }

        public long Class { get; set; }
    }
}
