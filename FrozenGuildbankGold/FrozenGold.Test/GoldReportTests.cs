using System;
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
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_GetsTaxTariff()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_GetsTransactionHistory()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_AllTransactionsIsSetFromDataSource()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_SetsLastUpdatedDate()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_ReceivedIsNull()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_SentToFrozbankIsNull()
        {
            Assert.True(false, "Todo");
        }

        [Fact]
        public void ctor_WhenCalled_PlayerReportsIsNull()
        {
            Assert.True(false, "Todo");
        }
    }
}