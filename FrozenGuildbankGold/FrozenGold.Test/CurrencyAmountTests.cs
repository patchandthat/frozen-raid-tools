using System;
using System.Runtime.Serialization;
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
        public void Equals_WhenCurrencyAmountsAreSame_ReturnsTrue()
        {
            uint totalcopper = 8923045;

            var first = CurrencyAmount.FromCopper(totalcopper);
            var second = CurrencyAmount.FromCopper(totalcopper);

            first.Equals(second).Should().BeTrue();
        }

        [Fact]
        public void Equals_whenCurrencyAmountsDiffer_ReturnsFalse()
        {
            uint firstCopper = 8923045;
            uint secondCopper = 89589564;

            var first = CurrencyAmount.FromCopper(firstCopper);
            var second = CurrencyAmount.FromCopper(secondCopper);

            first.Equals(second).Should().BeFalse();
        }

        [Fact]
        public void HashCode_SameForEquivalentObjects()
        {
            uint totalcopper = 8923045;

            var first = CurrencyAmount.FromCopper(totalcopper);
            var second = CurrencyAmount.FromCopper(totalcopper);

            first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Fact]
        public void HashCode_DifferentForDifferentQuantities()
        {
            uint firstCopper = 8923045;
            uint secondCopper = 89589564;

            var first = CurrencyAmount.FromCopper(firstCopper);
            var second = CurrencyAmount.FromCopper(secondCopper);

            first.GetHashCode().Should().NotBe(second.GetHashCode());
        }
        
        [Fact]
        public void OperatorEquals_WhenQuantitiesMatch_IsTrue()
        {
            uint totalcopper = 8923045;

            var first = CurrencyAmount.FromCopper(totalcopper);
            var second = CurrencyAmount.FromCopper(totalcopper);

            (first == second).Should().BeTrue();
        }
        
        [Fact]
        public void OperatorEquals_WhenQuantitiesDoNotMatch_IsFalse()
        {
            uint firstCopper = 8923045;
            uint secondCopper = 89589564;

            var first = CurrencyAmount.FromCopper(firstCopper);
            var second = CurrencyAmount.FromCopper(secondCopper);
            
            (first == second).Should().BeFalse();
        }

        [Fact]
        public void OperatorNotEqual_WhenQuantitiesMatch_IsFalse()
        {
            uint totalcopper = 8923045;

            var first = CurrencyAmount.FromCopper(totalcopper);
            var second = CurrencyAmount.FromCopper(totalcopper);
            
            (first != second).Should().BeFalse();
        }

        [Fact]
        public void OperatorNotEqual_WhenQuantitiesDoNotMatch_IsTrue()
        {
            uint firstCopper = 8923045;
            uint secondCopper = 89589564;

            var first = CurrencyAmount.FromCopper(firstCopper);
            var second = CurrencyAmount.FromCopper(secondCopper);
            
            (first != second).Should().BeTrue();
        }
        
        [Fact]
        public void OperatorAdd_WithTwoAmounts_ReturnsNewAmountWithCorrectValue()
        {
            var first = new CurrencyAmount(1, 2, 3);
            var second = new CurrencyAmount(4,9,6);
            
            var expected = new CurrencyAmount(5, 11, 9);

            (first + second).Should().Be(expected);
        }

        [Fact]
        public void OperatorSubtract_WithTwoAmounts_ReturnsNewAmountWithCorrectValue()
        {
            var first = new CurrencyAmount(1, 2, 3);
            var second = new CurrencyAmount(4,9,6);
            
            var expected = new CurrencyAmount(3, 7, 3);

            (second - first).Should().Be(expected);
        }

        [Fact]
        public void OperatorMultiply_WithIntegerMultiplier_ReturnsNewAmountWithCorrectValue()
        {
            var amount = new CurrencyAmount(1, 2, 3);
            int mult = 3;
            
            var expected = new CurrencyAmount(3, 6, 9);

            (amount * 3).Should().Be(expected);
        }
        
        [Fact]
        public void OperatorMultiply_WithIntegerMultiplier_IsCommutative()
        {
            var amount = new CurrencyAmount(1, 2, 3);
            int mult = 3;
            
            var expected = new CurrencyAmount(3, 6, 9);

            (3 * amount).Should().Be(expected);
        }

        [Fact]
        public void ToString_WhenCalled_ShouldOutputReadableFormat()
        {
            var sut = new CurrencyAmount(4, 3, 2);

            sut.ToString().Should().Be("4g 3s 2c");
        }
    }
}