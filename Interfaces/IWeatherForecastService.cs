using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MicroServiceTemplate.Interfaces
{
    public interface IWeatherForecastService
    {
        public Task<List<WeatherForecast>> GetWeatherForecasts(CancellationToken ct);
    }
}
