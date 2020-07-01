using System;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class FrozenTariffTests
    {
        [Fact]
        public void FrozenTariff_ShouldContainOneRate()
        {
            var sut = new FrozenTariff();

            sut.History.Length.Should().Be(1);
        }

        [Fact]
        public void FrozenTariff_FirstRate_ShouldBeginWeds24thJune2020()
        {
            var sut = new FrozenTariff();

            var startDate = sut.History[0].BeginsOn;

            startDate.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
            startDate.Day.Should().Be(24);
            startDate.Month.Should().Be(6);
            startDate.Year.Should().Be(2020);
        }

        [Fact]
        public void FrozenTariff_FirstRate_ShouldBe40gWeekly()
        {
            var sut = new FrozenTariff();

            sut.History[0].Amount.Should().Be(new CurrencyAmount(40, 0, 0));
            sut.History[0].RepeatInterval.Should().Be(TimeSpan.FromDays(7));
        }
    }
}