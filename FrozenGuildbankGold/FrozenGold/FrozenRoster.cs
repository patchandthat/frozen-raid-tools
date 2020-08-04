using System;

namespace FrozenGold
{
    public sealed class FrozenRoster : Roster
    {
        public FrozenRoster()
        {
            this
                .Add(new Player("Azgard", "Azgabank"))
                .Add(new Player("Balthassar", "Brokdoz", "Nathanial"))
                .Add(new Player("Wolfsbane"))

                .Add(new Player("Scorepan"))
                .Add(new Player("Horlof")
                {
                    LeftOn = new DateTimeOffset(2020, 07, 15, 1, 1, 1, TimeSpan.Zero)
                })
                .Add(new Player("Celehorn"))
                .Add(new Player("Xaosan", "Gundao"))
                .Add(new Player("Celee"))
                .Add(new Player("Phaedon"))
                .Add(new Player("Neffer", "Nefferbank"))
                .Add(new Player("Unboned"))
                .Add(new Player("Drunkan"))
                .Add(new Player("Demusske"))
                .Add(new Player("Dizzay"))
                .Add(new Player("Yrymyr")
                {
                    LeftOn = new DateTimeOffset(2020, 07, 07, 23, 59, 00, TimeSpan.Zero)
                })
                .Add(new Player("Kanaljen"))

                .Add(new Player("Akks")
                {
                    LeftOn = new DateTimeOffset(2020, 06, 25, 22, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Fixeh"))
                .Add(new Player("Marodören"))
                .Add(new Player("Rikxx"))
                .Add(new Player("Whi", "Whilerr"))
                .Add(new Player("Deceptions"))
                .Add(new Player("Raige", "Bankaige"))
                .Add(new Player("Poutana", "Poutanara")
                {
                    JoinedOn = new DateTimeOffset(2020, 07, 15, 1, 1, 1, TimeSpan.Zero)
                })
                .Add(new Player("Jione"))
                .Add(new Player("Týr"))
                .Add(new Player("Strongstra"))
                .Add(new Player("Jawewarrior")
                {
                    LeftOn = new DateTimeOffset(2020, 07, 15, 1, 1, 1, TimeSpan.Zero)
                })
                .Add(new Player("Eresant", "Cowwithagun", "Travelform"))
                .Add(new Player("Elcabra")
                {
                    // On leave
                    JoinedOn = DateTimeOffset.Now.AddDays(14)
                })
                .Add(new Player("Moonrock")
                {
                    JoinedOn = new DateTimeOffset(2020, 07, 15, 1, 1, 1, TimeSpan.Zero)
                })
                .Add(new Player("Pykkles"))
                .Add(new Player("Onahawe", "Aijin"))
                .Add(new Player("Lauwl")
                {
                    JoinedOn = DateTimeOffset.Now.AddDays(14)
                })

                .Add(new Player("Fesha"))
                .Add(new Player("Enslave"))
                .Add(new Player("Spidle"))
                .Add(new Player("Exspes"))
                .Add(new Player("Ceeben", "Lähitapiola"))
                .Add(new Player("Kuzuri"))
                .Add(new Player("Darchi", "Darchibank"))
                .Add(new Player("Bezbani"))
                .Add(new Player("Firelordozai"))
                .Add(new Player("Heuwmauw"))
                .Add(new Player("Frostmon"))
                .Add(new Player("Anthraxx"))
                .Add(new Player("Stompalomp"))
                .Add(new Player("Buksy")
                {
                    LeftOn = new DateTimeOffset(2020, 06, 25, 22, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Stellagosa")
                {
                    // Social
                    JoinedOn = DateTimeOffset.Now.AddDays(14)
                })
                .Add(new Player("Gicanu")
                {
                    LeftOn = new DateTimeOffset(2020, 06, 25, 22, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Awesomekek")
                {
                    JoinedOn = new DateTimeOffset(2020, 08, 05, 06, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Boibard")
                {
                    JoinedOn = new DateTimeOffset(2020, 08, 05, 06, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Aswe")
                {
                    JoinedOn = new DateTimeOffset(2020, 08, 05, 06, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Spinogriz")
                {
                    JoinedOn = new DateTimeOffset(2020, 08, 05, 06, 0, 0, TimeSpan.Zero)
                })
                .Add(new Player("Maniya")
                {
                    JoinedOn = new DateTimeOffset(2020, 08, 05, 06, 0, 0, TimeSpan.Zero)
                });
        }
    }
}