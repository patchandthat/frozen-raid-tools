using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace FrozenGold.tests
{
    public class GoldReportTests
    {
        private IDataSource _dataSource;

        public GoldReportTests()
        {
            _dataSource = A.Fake<IDataSource>();
        }

        private GoldReport CreateSut()
        {
            return new GoldReport(_dataSource);
        }
        
        [Fact]
        public void ctor_CalledWithNullDataSource_ThrowsArgumentNullException()
        {
            _dataSource = null;

            Action create = () => CreateSut();

            create.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_WhenCalled_GetsGuildRoster()
        {
            var sut = CreateSut();

            A.CallTo(() => _dataSource.GetRoster())
                .MustHaveHappened();
        }

        [Fact]
        public void ctor_WhenCalled_GetsTaxTariff()
        {
            var sut = CreateSut();

            A.CallTo(() => _dataSource.GetTariff())
                .MustHaveHappened();
        }

        [Fact]
        public void ctor_WhenCalled_GetsTransactionHistory()
        {
            var sut = CreateSut();

            A.CallTo(() => _dataSource.GetTransactionHistory())
                .MustHaveHappened();
        }

        [Fact]
        public void ctor_WhenCalled_AllTransactionsIsSetFromDataSource()
        {
            var now = DateTimeOffset.UtcNow;
            IReadOnlyList<Transaction> transactions = new List<Transaction>()
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "FrozenGold",
                    now.AddDays(-2)),
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "FrozenGold",
                    "FrozBank",
                    now.AddDays(-1))
            };
            A.CallTo(() => _dataSource.GetTransactionHistory())
                .Returns(transactions);

            var sut = CreateSut();

            sut.AllTransactions.Should().BeEquivalentTo(transactions);
        }

        [Fact]
        public void ctor_WhenCalled_SetsLastUpdatedDate()
        {
            DateTimeOffset lastUpdateDate = DateTimeOffset.UtcNow;
            A.CallTo(() => _dataSource.GetLastUpdatedDate())
                .Returns(lastUpdateDate);

            var sut = CreateSut();

            sut.LastUpdated.Should().Be(lastUpdateDate);
        }

        [Fact]
        public void ctor_WhenCalled_ReceivedIsZero()
        {
            var sut = CreateSut();

            sut.Received.Should().Be(CurrencyAmount.Zero);
        }

        [Fact]
        public void ctor_WhenCalled_SentToFrozbankIsZero()
        {
            var sut = CreateSut();

            sut.SentToBanker.Should().Be(CurrencyAmount.Zero);
        }

        [Fact]
        public void ctor_WhenCalled_GoldOnHandIsZero()
        {
            var sut = CreateSut();

            sut.GoldOnHand.Should().Be(CurrencyAmount.Zero);
        }

        [Fact]
        public void ctor_WhenCalled_PlayerReportsIsEmpty()
        {
            var sut = CreateSut();

            sut.PlayerReports.Should().BeEmpty();
        }

        [Fact]
        public void BuildReport_WhenCalled_AddsPlayerReportForEachPlayerInRoster()
        {
            var roster = new Roster()
                .Add(new Player("Jiwari"))
                .Add(new Player("Pykkles"))
                .Add(new Player("Fesha"));
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            A.CallTo(() => _dataSource.GetRoster())
                .Returns(roster);
            A.CallTo(() => _dataSource.GetTariff())
                .Returns(tariff);

            var sut = CreateSut();
            
            sut.BuildReport();

            sut.PlayerReports.Count().Should().Be(roster.Players.Count());

            foreach (Player player in roster.Players)
            {
                sut.PlayerReports
                    .SingleOrDefault(pr => pr.Player == player)
                    .Should().NotBeNull();
            }
        }

        [Fact]
        public void BuildReport_WhenPlayerJoinedBeforeTariffStartDate_CalculatesTaxDueToPresentDate()
        {
            var roster = new Roster()
                .Add(new Player("Neffer"));
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };
            var now = new DateTimeOffset(2020, 07, 3, 0, 0, 0, TimeSpan.FromHours(1));

            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);

            var sut = CreateSut();
            sut.BuildReport();

            var neffer = sut.PlayerReports.Single();

            neffer.AmountDueToDate.Should().Be(CurrencyAmount.FromGold(80));
        }

        [Fact]
        public void WhenPlayerJoinedAfterTaxStarted_CalculatesTaxForWeeksOnTheRoster()
        {
            var roster = new Roster()
                .Add(new Player("Neffer")
                {
                    JoinedOn = new DateTimeOffset(2020, 07, 1, 0, 0, 0, TimeSpan.Zero)
                });
            
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };
            var now = new DateTimeOffset(2020, 07, 3, 0, 0, 0, TimeSpan.FromHours(1));

            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);

            var sut = CreateSut();
            sut.BuildReport();
            
            var neffer = sut.PlayerReports.Single();

            neffer.AmountDueToDate.Should().Be(CurrencyAmount.FromGold(40));
        }
        
        [Fact]
        public void BuildReport_WhenPlayerHasRetired_CalculatesTaxDueUntilRetirementDate()
        { 
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1)),
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };
            
            var roster = new Roster()
                .Add(new Player("Neffer")
                {
                    RetiredOn = tariff.CurrentRate.BeginsOn.AddDays(9)
                });
            
            var now =
                tariff.CurrentRate.BeginsOn.AddMonths(2);
                
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);

            var sut = CreateSut();
            sut.BuildReport();
            
            var neffer = sut.PlayerReports.Single();

            neffer.AmountDueToDate.Should().Be(CurrencyAmount.FromGold(40));
        }

        [Fact]
        public void BuildReport_WhenTariffHasMultipleRates_CalculatesTaxDueForEachTimePeriodToCurrentDate()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    },
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(20),
                        BeginsOn = firstRateStart.AddDays(21),
                        RepeatInterval = TimeSpan.FromDays(7)
                    },
                }
            };
            
            var roster = new Roster()
                .Add(new Player("Neffer"));
            
            // 3 weeks of 40g, 5 weeks of 20g
            var now = firstRateStart.AddDays(7 * 8);
            CurrencyAmount expectedAmountDue = CurrencyAmount.FromGold(220);
                
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            
            var sut = CreateSut();
            sut.BuildReport();
            
            var neffer = sut.PlayerReports.Single();

            neffer.AmountDueToDate.Should().Be(expectedAmountDue);
        }

        [Fact]
        public void BuildReport_WithMultipleRatesAndPlayers_CalculatesCorrectAmountDueForEachPlayer()
        {
            Assert.True(false, "Write me");
        }
    }
}