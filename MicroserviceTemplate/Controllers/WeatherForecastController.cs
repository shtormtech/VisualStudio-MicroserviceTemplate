using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using $safeprojectname$.Interfaces;
using $safeprojectname$.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService weatherForecastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            this.weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WeatherForecast>>> Get(CancellationToken ct)
        {
            _logger.LogInformation($"GET /WeatherForecast");
            return await weatherForecastService.GetWeatherForecasts(ct);
        }
    }
}
