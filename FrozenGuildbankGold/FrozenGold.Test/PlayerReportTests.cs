using System;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class PlayerReportTests
    {
        private Player _player;
        private TariffItem _tariffItem;
        private Tariff _tariff;

        public PlayerReportTests()
        {
            _player = new Player("Neffer");
            
            _tariffItem = new TariffItem
            {
                Amount = CurrencyAmount.FromGold(40),
                BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                RepeatInterval = TimeSpan.FromDays(7)
            };
            _tariff = new Tariff()
            {
                History = new[]{_tariffItem}
            };
        }

        private PlayerReport CreateSut()
        {
            return new PlayerReport(_player, _tariff);
        }

        [Fact]
        public void ctor_CalledWithNullPlayer_ThrowsArgumentNullException()
        {
            _player = null;
            
            Action ctorAction = () => CreateSut();

            ctorAction.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_CalledWithNullTariff_ThrowsArgumentNullException()
        {
            _tariff = null;
            
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

        [Fact]
        public void PaymentStartDate_WhenPlayerJoinedBeforeRatesStarted_FirstPaymentDateIsWhenRatesStart()
        {
            DateTimeOffset joinDate = DateTimeOffset.Now.AddDays(-21);
            DateTimeOffset startDate = DateTimeOffset.Now.AddDays(-5);
            
            _player = new Player("Anthraxx")
            {
                JoinedOn = joinDate
            };
            
            _tariff = new Tariff()
            {
                History = new []{ new TariffItem()
                {
                    Amount = CurrencyAmount.FromGold(1),
                    BeginsOn = startDate,
                }, }
            };

            var sut = CreateSut();

            sut.PaymentsStartFrom.Should().Be(startDate);
        }

        [Fact]
        public void PaymentStartDate_WhenPlayerJoinedAfterRatesStarted_FirstPaymentDateIsWhenPlayerJoined()
        {
            DateTimeOffset startDate = DateTimeOffset.Now.AddDays(-21);
            DateTimeOffset joinDate = DateTimeOffset.Now.AddDays(-5);

            _player = new Player("Anthraxx")
            {
                JoinedOn = joinDate
            };

            _tariff = new Tariff()
            {
                History = new[]
                {
                    new TariffItem()
                    {
                        Amount = CurrencyAmount.FromGold(1),
                        BeginsOn = startDate,
                    },
                }
            };

            var sut = CreateSut();

            sut.PaymentsStartFrom.Should().Be(joinDate);
        }
    }
}