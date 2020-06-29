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
                    LeftOn = tariff.CurrentRate.BeginsOn.AddDays(9)
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
        public void BuildReport_WithMultipleRatesAndPlayersWithDifferentMembershipDurations_CalculatesCorrectAmountDueForEachPlayer()
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
                .Add(new Player("Neffer"))
                .Add(new Player("Demusske")
                {
                    JoinedOn = firstRateStart.AddDays(7)
                })
                .Add(new Player("Dizzay")
                {
                    LeftOn = firstRateStart.AddDays(28) 
                })
                .Add(new Player("Unboned")
                {
                    LeftOn = firstRateStart.AddDays(-5)
                });
            
            // 3 weeks of 40g, 5 weeks of 20g
            var now = firstRateStart.AddDays(7 * 8);
            CurrencyAmount nefferExpectedDue = CurrencyAmount.FromGold(220);
            CurrencyAmount demusskeExpectedDue = CurrencyAmount.FromGold(180);
            CurrencyAmount dizzayExpectedDue = CurrencyAmount.FromGold(140);
            CurrencyAmount unbonedExpectedDue = CurrencyAmount.FromGold(0);
                
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            
            var sut = CreateSut();
            sut.BuildReport();
            
            var neffer = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Neffer");
            var demusske = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Demusske");
            var dizzay = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Dizzay");
            var unboned = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Unboned");

            neffer.AmountDueToDate.Should().Be(nefferExpectedDue);
            demusske.AmountDueToDate.Should().Be(demusskeExpectedDue);
            dizzay.AmountDueToDate.Should().Be(dizzayExpectedDue);
            unboned.AmountDueToDate.Should().Be(unbonedExpectedDue);
        }

        [Fact]
        public void BuildReport_WhenRosterMainCharacterSentMoneyToTaxman_AddsToTotalForPlayer()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            var neffer = sut.PlayerReports.Single();

            neffer.AmountPaid.Should().Be(CurrencyAmount.FromGold(80));
        }
        
        [Fact]
        public void BuildReport_WhenRosterAltCharacterSentMoneyToTaxman_AddsToTotalForPlayer()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer", "Confused", "Marnaa"))
                .Add(new Player("Jiwari"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Marnaa",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Marnaa",
                    "Frozengold",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            var neffer = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Neffer");

            neffer.AmountPaid.Should().Be(CurrencyAmount.FromGold(80));
        }
        
        [Fact]
        public void BuildReport_WhenRosterCharacterSentMoneyToTaxman_AddsToTotalReceived()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer", "Confused", "Marnaa"))
                .Add(new Player("Jiwari"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Marnaa",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(10), 
                    "Jiwari",
                    "Frozengold",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            sut.Received.Should().Be(CurrencyAmount.FromGold(50));
        }

        [Fact]
        public void BuildReport_WhenTaxmanSentMoneyToBanker_AddsToTotalSent()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(20), 
                    "Frozengold",
                    "Frozbank",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            sut.SentToBanker.Should().Be(CurrencyAmount.FromGold(20));
        }

        [Fact]
        public void ctor_WhenCalled_MailboxFeesIsZero()
        {
            var sut = CreateSut();

            sut.MailboxFees.Should().Be(CurrencyAmount.Zero);
        }
        
        [Fact]
        public void BuildReport_WhenMoneySentToBanker_MailboxFeesIncreaseBy30Copper()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(20), 
                    "Frozengold",
                    "Frozbank",
                    firstRateStart.AddDays(9)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(5), 
                    "Frozengold",
                    "Frozbank",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();
            
            sut.MailboxFees.Should().Be(CurrencyAmount.FromCopper(60));
        }

        [Fact]
        public void BuildReport_WhenMoneyReceivedAndSentToBanker_GoldOnHandIsCorrectWithMailboxFees()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"));

            IReadOnlyList<Transaction> transactions = new []
            {
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(1)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(20), 
                    "Frozengold",
                    "Frozbank",
                    firstRateStart.AddDays(9)), 
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(5), 
                    "Frozengold",
                    "Frozbank",
                    firstRateStart.AddDays(9)), 
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            sut.GoldOnHand.Should().Be(new CurrencyAmount(14, 99, 40));
        }

        [Fact]
        public void BuildReport_WhenPlayerNotInRosterSentGoldToTaxman_AddsToCollectionOfOddTransactions()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(14);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"));

            Transaction oddTransaction = new Transaction(
                TransactionType.MoneyTransfer,
                CurrencyAmount.FromGold(40),
                "Jiwari",
                "Frozengold",
                firstRateStart.AddDays(1));
            IReadOnlyList<Transaction> transactions = new []
            {
                oddTransaction
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();
            
            var neffer = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Neffer");

            neffer.AmountPaid.Should().Be(CurrencyAmount.Zero);

            sut.OddTransactions.Count().Should().Be(1);
            sut.OddTransactions.Should().Contain(oddTransaction);
        }

        [Fact]
        public void ctor_WhenCalled_RefundsIsEmpty()
        {
            var sut = CreateSut();

            sut.Refunds.Should().BeEmpty();
        }
        
        [Fact]
        public void BuildReport_RefundsToPlayersWhoHaveLeft_ShouldNotBeOddTransactions()
        {
            var firstRateStart = new DateTimeOffset(2020, 06, 24, 0, 0, 0, TimeSpan.FromHours(1));
            var now = firstRateStart.AddDays(7);
            var tariff = new Tariff
            {
                History = new []
                {
                    new TariffItem
                    {
                        Amount = CurrencyAmount.FromGold(40),
                        BeginsOn = firstRateStart,
                        RepeatInterval = TimeSpan.FromDays(7)
                    }
                }
            };

            var roster = new Roster()
                .Add(new Player("Neffer"))
                .Add(new Player("Gicanu")
                {
                    // Left during week 2
                    LeftOn = firstRateStart.AddDays(9)
                });

            var refundTxn = new Transaction( 
                TransactionType.MoneyTransfer,
                CurrencyAmount.FromGold(40), 
                "Frozengold",
                "Gicanu",
                firstRateStart.AddDays(10));
            
            IReadOnlyList<Transaction> transactions = new []
            {
                // Income week 1
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddHours(12)),
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Gicanu",
                    "Frozengold",
                    firstRateStart.AddHours(12)), 
                // Income week 2
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Neffer",
                    "Frozengold",
                    firstRateStart.AddDays(8)),
                new Transaction(
                    TransactionType.MoneyTransfer,
                    CurrencyAmount.FromGold(40), 
                    "Gicanu",
                    "Frozengold",
                    firstRateStart.AddDays(8)),
                // Refund
                refundTxn,
            };
            
            A.CallTo(() => _dataSource.GetRoster()).Returns(roster);
            A.CallTo(() => _dataSource.GetTariff()).Returns(tariff);
            A.CallTo(() => _dataSource.NowServerTime).Returns(now);
            A.CallTo(() => _dataSource.GetTransactionHistory()).Returns(transactions);

            var sut = CreateSut();
            
            sut.BuildReport();

            sut.Received.Should().Be(CurrencyAmount.FromGold(160));
            sut.Refunded.Should().Be(CurrencyAmount.FromGold(40));
            sut.GoldOnHand.Should().Be(CurrencyAmount.FromGold(120));
            sut.OddTransactions.Should().BeEmpty();
            sut.Refunds.Count().Should().Be(1);
            sut.Refunds.Should().Contain(refundTxn);
            
            var gicanu = sut.PlayerReports.Single(pr => pr.Player.Main.Name == "Gicanu");
            gicanu.AmountPaid.Should().Be(CurrencyAmount.FromGold(40));
            gicanu.AmountDueToDate.Should().Be(CurrencyAmount.FromGold(40));
        }
    }
}