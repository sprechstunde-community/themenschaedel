using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Themenschaedel.API.Services;
using Themenschaedel.Shared.Models;

namespace Themenschaedel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : ControllerBase
    {
        private readonly IDatabaseService _database;
        private readonly ILogger<HostController> _logger;

        public HostController(ILogger<HostController> logger, IDatabaseService databaseService)
        {
            _database = databaseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetAllPeople()
        {
            try
            {
                return Ok(await _database.GetAllPeopleAsync());
            }
            catch (Exception e)
            {
                _logger.LogError("Error while trying to return all people. Error:\n" + e.Message);
                SentrySdk.CaptureException(e);
            }

            return Problem();
        }
    }
}
