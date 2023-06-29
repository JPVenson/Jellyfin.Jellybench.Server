using Jellyfin.Jellybench.Database;
using Jellyfin.Jellybench.Server.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Jellybench.Server.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ChartApiController : ControllerBase
    {
        private readonly IDbContextFactory<JellybenchDataContext> _dbContextFactory;

        public ChartApiController(IDbContextFactory<JellybenchDataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        ///     Gets the series of measured Streams for Cpus.
        /// </summary>
        /// <returns>A chart dataset for the CPU measured values</returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChartDataSeries<int>))]
        public async ValueTask<IActionResult> GetCpuChartValues()
        {
            await using var jellybenchDataContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            var groupBy = await jellybenchDataContext.DataPoints.GroupBy(e => e.BenchRun.CpuName).ToArrayAsync().ConfigureAwait(false);
            var result = new ChartDataSeries<int>
            {
                Labels = groupBy.Select(e => e.Key).ToArray(),
                Values = groupBy.Select(f => (int)Math.Floor(f.Average(e => e.NumberOfStreams))).ToArray()
            };

            return Ok(result);
        }
    }
}
