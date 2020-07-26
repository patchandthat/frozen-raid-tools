using System;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FrozenGold.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ILogger<TransactionsController> logger, IDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [EnableCors]
        [HttpGet]
        public TransactionHistory Get()
        {
            try
            {
                var transactions = _dataSource
                    .GetTransactionHistory()
                    .Select(tx => new TransactionDto
                    {
                        WhenServerTime = tx.WhenServerTime,
                        CopperAmount = tx.Amount.TotalCopper,
                        PlayerFrom = tx.PlayerFrom,
                        PlayerTo = tx.PlayerTo,
                        Type = tx.Type
                    })
                    .ToArray();

                return new TransactionHistory
                {
                    LastUpdated = _dataSource.GetLastUpdatedDate(),
                    Transactions = transactions
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to provide transactions");
                throw;
            }
        }
    }
}
