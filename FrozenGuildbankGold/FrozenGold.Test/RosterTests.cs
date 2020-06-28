using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class RosterTests
    {
        private Roster CreateSut()
        {
            return new Roster();
        }
        
        [Fact]
        public void ctor_WhenCalled_PlayersIsEmpty()
        {
            var sut = CreateSut();

            sut.Players.Should().BeEmpty();
        }

        [Fact]
        public void Add_WhenCalled_AddsPlayerToCollection()
        {
            var player = new Player("Neff");
            
            var sut = CreateSut();

            sut.Add(player);

            sut.Players.Count().Should().Be(1);
            sut.Players.Single().Should().Be(player);
        }

        [Fact]
        public void Add_WhenCalled_ReturnsSameRosterInstance()
        {
            var player = new Player("Neff");
            
            var sut = CreateSut();

            var result = sut.Add(player);

            result.Should().BeSameAs(sut);
        }

        [Fact]
        public void Add_OnAddingMultiplePlayers_AllExistInCollection()
        {
            var one = new Player("Neff");
            var two = new Player("Jeff");
            var three = new Player("Geoff");

            var sut = CreateSut();

            sut
                .Add(one)
                .Add(two)
                .Add(three);

            sut.Players.Count().Should().Be(3);

            sut.Players.Should().Contain(one);
            sut.Players.Should().Contain(two);
            sut.Players.Should().Contain(three);
        }

        [Fact]
        public void Add_OnAddDuplicate_ThrowsException()
        {
            var player = new Player("whatever");

            var sut = CreateSut();

            Action addAction = () => sut.Add(player);

            sut.Add(player);

            addAction.Should().Throw<DuplicateException>();
        }

        [Fact]
        public void Find_WhenRosterIsEmpty_ReturnsNull()
        {
            var sut = CreateSut();

            var result = sut.Find("Neffer");

            result.Should().BeNull();
        }

        [Fact]
        public void Find_WhenPlayerExistsInRoster_FindsPlayer()
        {
            var neff = new Player("Neffer");
            var ji = new Player("Jithree");
            var akks = new Player("Akks");
            
            var sut = CreateSut();

            sut
                .Add(neff)
                .Add(ji)
                .Add(akks);

            var result = sut.Find("Akks");

            result.Should().BeSameAs(akks);
        }

        [Fact]
        public void Find_WhenCharacterIsAltOfPlayer_FindsPlayer()
        {
            var neff = new Player("Neffer", "Marnaa", "Confused", "Nefferbank");

            var sut = CreateSut()
                .Add(neff);
            
            var result = sut.Find("Confused");

            result.Should().BeSameAs(neff);
        }
    }
}