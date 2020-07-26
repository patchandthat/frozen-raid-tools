using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FrozenGold.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RosterController : ControllerBase
    {
        private readonly IDataSource _dataSource;
        private readonly ILogger<RosterController> _logger;

        public RosterController(ILogger<RosterController> logger, IDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [EnableCors]
        [HttpGet]
        public Roster Get()
        {
            try
            {
                return _dataSource.GetRoster();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to provide roster");
                throw;
            }
        }
    }
}