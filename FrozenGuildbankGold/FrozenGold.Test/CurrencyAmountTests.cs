using System;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class CurrencyAmountTests
    {
        private CurrencyAmount CreateSut(in uint gold, in uint silver, in uint copper)
        {
            return new CurrencyAmount(gold, silver, copper);
        }
        
        [Fact]
        public void ctor_CalledWithGSC_SetsGSC()
        {
            uint gold = 123;
            uint silver = 45;
            uint copper = 56;

            var sut = CreateSut(gold, silver, copper);

            sut.Gold.Should().Be(gold);
            sut.Silver.Should().Be(silver);
            sut.Copper.Should().Be(copper);
        }

        [Fact]
        public void ctor_CalledOver100Silver_CarriesToGold()
        {
            uint gold = 123;
            uint silver = 145;
            uint copper = 91;

            var sut = CreateSut(gold, silver, copper);

            sut.Gold.Should().Be(124);
            sut.Silver.Should().Be(45);
            sut.Copper.Should().Be(copper);
        }

        [Fact]
        public void ctor_CalledWithOver100Copper_CarriesToSilver()
        {
            uint gold = 10;
            uint silver = 45;
            uint copper = 259;

            var sut = CreateSut(gold, silver, copper);

            sut.Gold.Should().Be(10);
            sut.Silver.Should().Be(45 + 2);
            sut.Copper.Should().Be(59);
        }

        [Fact]
        public void ctor_CalledWithOver10000Copper_CarriesToSilverAndGold()
        {
            uint gold = 10;
            uint silver = 45;
            uint copper = 83259;

            var sut = CreateSut(gold, silver, copper);

            sut.Gold.Should().Be(10 + 8);
            sut.Silver.Should().Be(45 + 32);
            sut.Copper.Should().Be(59);
        }

        [Fact]
        public void ctor_CalledWithOver100CopperAndSilver_CarriesToSilverAndGold()
        {
            uint gold = 10;
            uint silver = 195;
            uint copper = 799;

            var sut = CreateSut(gold, silver, copper);

            sut.Gold.Should().Be(12);
            sut.Silver.Should().Be(2);
            sut.Copper.Should().Be(99);
        }

        [Theory]
        [InlineData(84, 0, 0, 84)]
        [InlineData(2495, 0, 24, 95)]
        [InlineData(7397, 0, 73, 97)]
        [InlineData(15764, 1, 57, 64)]
        [InlineData(95736, 9, 57, 36)]
        [InlineData(1579535, 157, 95, 35)]
        public void FromCopper_WhenCalled_ShouldCreateCorrectAmount(
            uint copper, uint expectedGold, uint expectedSilver, uint expectedCopper)
        {
            var sut = CurrencyAmount.FromCopper(copper);

            sut.Gold.Should().Be(expectedGold);
            sut.Silver.Should().Be(expectedSilver);
            sut.Copper.Should().Be(expectedCopper);
        }

        [Fact]
        public void TotalCopper_WhenCalled_ShouldBeCorrect()
        {
            uint totalCopper = 1579535;
            uint gold = 157;
            uint silver = 95;
            uint copper = 35;
            
            var sut = CurrencyAmount.FromCopper(totalCopper);

            sut.Gold.Should().Be(gold);
            sut.Silver.Should().Be(silver);
            sut.Copper.Should().Be(copper);

            sut.TotalCopper.Should().Be(totalCopper);
        }

        [Fact]
        public void IEquatable()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void HashCode()
        {
            Assert.True(false, "Todo");
        }
        
        [Fact]
        public void OperatorAdd()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void OperatorSubtract()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ToString_WhenCalled_ShouldOutputReadableFormat()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void OperatorMult()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void OperatorDiv()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void OperatorEquals()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void OperatorNotEqual()
        {
            Assert.True(false, "Todo");
        }
    }
}