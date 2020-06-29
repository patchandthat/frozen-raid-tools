using System;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class PlayerReportTests
    {
        private Player _player;
        private TariffItem _tariffItem;

        public PlayerReportTests()
        {
            _player = new Player("Neffer");
            
            _tariffItem = new TariffItem
            {
                Amount = CurrencyAmount.FromGold(40),
                BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                RepeatInterval = TimeSpan.FromDays(7)
            };
        }

        private PlayerReport CreateSut()
        {
            return new PlayerReport(_player, _tariffItem);
        }

        [Fact]
        public void ctor_CalledWithNullPlayer_ThrowsArgumentNullException()
        {
            _player = null;
            
            Action ctorAction = () => CreateSut();

            ctorAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_CalledWithNullTariffItem_ThrowsArgumentNullException()
        {
            _tariffItem = null;
            
            Action ctorAction = () => CreateSut();
            
            ctorAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReportSummary_WhenPlayerHasPaidExactAmountDue_StatusIsPaidInFullAndZeroWeeksDifference()
        {
            var sut = CreateSut();
            
            sut.AmountPaid = CurrencyAmount.FromGold(40);
            sut.AmountDueToDate = CurrencyAmount.FromGold(40);

            var summary = sut.ReportSummary;

            summary.Status.Should().Be(PlayerPaymentStatus.PaidInFull);
            summary.WeeksDifference.Should().Be(0);
        }

        [Fact]
        public void ReportSummary_WhenPlayerHasPaidMoreThanDueByExactWeeklyAmount_StatusIsAheadAndNumberOfFullWeeksIndicated()
        {
            var sut = CreateSut();
            
            sut.AmountPaid = CurrencyAmount.FromGold(120);
            sut.AmountDueToDate = CurrencyAmount.FromGold(40);

            var summary = sut.ReportSummary;

            summary.Status.Should().Be(PlayerPaymentStatus.Ahead);
            summary.WeeksDifference.Should().Be(2);
        }
        
        [Fact]
        public void ReportSummary_WhenPlayerHasPaidMoreThanDue_StatusIsAheadAndNumberOfFullWeeksIndicatedRoundedDown()
        {
            var sut = CreateSut();
            
            sut.AmountPaid = CurrencyAmount.FromGold(90);
            sut.AmountDueToDate = CurrencyAmount.FromGold(40);

            var summary = sut.ReportSummary;

            summary.Status.Should().Be(PlayerPaymentStatus.Ahead);
            summary.WeeksDifference.Should().Be(1);
        }

        [Fact]
        public void ReportSummary_WhenPlayerHasPaidLessThanDueByExactWeeklyAmount_StatusIsBehindAndNumberOfFullWeeksIndicated()
        {
            var sut = CreateSut();
            
            sut.AmountPaid = CurrencyAmount.FromGold(40);
            sut.AmountDueToDate = CurrencyAmount.FromGold(80);

            var summary = sut.ReportSummary;

            summary.Status.Should().Be(PlayerPaymentStatus.Behind);
            summary.WeeksDifference.Should().Be(1);
        }

        [Fact]
        public void ReportSummary_WhenPlayerHasPaidLessThanDue_StatusIsBehindAndNumberOfFullWeeksIndicatedRoundedUp()
        {
            var sut = CreateSut();
            
            sut.AmountPaid = CurrencyAmount.FromGold(50);
            sut.AmountDueToDate = CurrencyAmount.FromGold(120);

            var summary = sut.ReportSummary;

            summary.Status.Should().Be(PlayerPaymentStatus.Behind);
            summary.WeeksDifference.Should().Be(2);
        }
    }
}