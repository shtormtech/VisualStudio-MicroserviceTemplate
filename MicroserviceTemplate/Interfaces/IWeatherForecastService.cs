using $safeprojectname$.Models;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace $safeprojectname$.Interfaces
{
    public interface IWeatherForecastService
    {
        public Task<List<WeatherForecast>> GetWeatherForecasts(CancellationToken ct);
    }
}
