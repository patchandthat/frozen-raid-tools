using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FrozenGold.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TariffController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<TariffController> _logger;

        public TariffController(ILogger<TariffController> logger, IDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [EnableCors]
        [HttpGet]
        public Tariff Get()
        {
            try
            {
                return _dataSource.GetTariff();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to provide tariff");
                throw;
            }
        }
    }
}