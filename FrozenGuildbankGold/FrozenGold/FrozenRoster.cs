using System;

namespace FrozenGold
{
    public class FrozenRoster : Roster
    {
        public FrozenRoster()
        {
            this
                .Add(new Player("Azgard", "Azgabank"))
                .Add(new Player("Balthassar"))
                .Add(new Player("Wolfsbane"))

                .Add(new Player("Scorepan"))
                .Add(new Player("Horlof"))
                .Add(new Player("Celehorn"))
                .Add(new Player("Xaosan", "Gundao"))
                .Add(new Player("Celee"))
                .Add(new Player("Phaedon"))
                .Add(new Player("Neffer"))
                .Add(new Player("Unboned"))
                .Add(new Player("Drunkan"))
                .Add(new Player("Demusske"))
                .Add(new Player("Dizzay"))
                .Add(new Player("Yrymyr"))
                .Add(new Player("Kanaljen"))

                .Add(new Player("Fixeh"))
                .Add(new Player("Marodören"))
                .Add(new Player("Rikxx"))
                .Add(new Player("Whi"))
                .Add(new Player("Deceptions"))
                .Add(new Player("Raige"))
                .Add(new Player("Poutana"))
                .Add(new Player("Jione"))
                .Add(new Player("Akks"))
                .Add(new Player("Týr"))
                .Add(new Player("Strongstra"))
                .Add(new Player("Jawewarrior"))
                .Add(new Player("Eresant"))
                .Add(new Player("Elcabra"))
                .Add(new Player("Moonrock"))
                .Add(new Player("Pykkles"))
                .Add(new Player("Onahawe"))

                .Add(new Player("Fesha"))
                .Add(new Player("Enslave"))
                .Add(new Player("Spidle"))
                .Add(new Player("Exspes"))
                .Add(new Player("Ceeben", "Lähitapiola"))
                .Add(new Player("Kuzuri"))
                .Add(new Player("Darchi"))
                .Add(new Player("Bezbani"))
                .Add(new Player("Firelordozai"))
                .Add(new Player("Heuwmauw"))
                .Add(new Player("Frostmon"))
                .Add(new Player("Anthraxx"))
                .Add(new Player("Stompalomp"))
                .Add(new Player("Buksy"))
                .Add(new Player("Gicanu")
                {
                    LeftOn = new DateTimeOffset(2020, 06, 25, 22, 0, 0, TimeSpan.Zero)
                });
        }
    }
}