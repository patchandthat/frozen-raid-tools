using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrozenGold.Web.Services
{
    public class RosterBuilder
    {
        private PlayersDto _dto;

        internal RosterBuilder(PlayersDto dto)
        {
            _dto = dto;
        }

        internal Roster Build()
        {
            var roster = new Roster();

            foreach (var p in _dto.Players)
            {
                var fgp = new FrozenGold.Player(
                    p.Main.Name, 
                    p.Alts.Select(c => c.Name).ToArray());

                fgp.JoinedOn = p.JoinedOn ?? DateTimeOffset.MinValue;
                fgp.LeftOn = p.LeftOn;

                roster.Add(fgp);
            }

            return roster;
        }
    }
}
