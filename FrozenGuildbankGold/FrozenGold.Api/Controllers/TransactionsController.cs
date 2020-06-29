using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public TransactionHistory Get()
        {
            try
            {
                return new TransactionHistory
                {
                    LastUpdated = _dataSource.GetLastUpdatedDate(),
                    Transactions = _dataSource.GetTransactionHistory().ToArray()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to provode transactions");
                throw;
            }
        }
    }
}
