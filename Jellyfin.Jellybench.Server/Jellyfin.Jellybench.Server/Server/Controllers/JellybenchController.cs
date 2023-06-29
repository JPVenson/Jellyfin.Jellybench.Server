using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jellyfin.Jellybench.Database;
using Jellyfin.Jellybench.Database.Entities;
using Jellyfin.Jellybench.Server.Server.Services.RequestKey;
using Jellyfin.Jellybench.Server.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Jellybench.Server.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class JellybenchController : ControllerBase
{
    private readonly IDbContextFactory<JellybenchDataContext> _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IRequestKeyGeneratorService _requestKeyGeneratorService;

    public JellybenchController(IDbContextFactory<JellybenchDataContext> dbContextFactory,
        IMapper mapper,
        IRequestKeyGeneratorService requestKeyGeneratorService)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _requestKeyGeneratorService = requestKeyGeneratorService;
    }

    [HttpGet]
    [Route("[action]")]
    [EnableRateLimiting("UploadLimiter")]
    public async ValueTask<IActionResult> PrepareDataPoints()
    {
        await using var jellybenchDataContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var request = new JellybenchRequest()
        {
            DataPoints = await _mapper.ProjectTo<JellybenchRequestDataPoint>(jellybenchDataContext.RequestDataPoints).ToArrayAsync().ConfigureAwait(false),
            RequestKey = await _requestKeyGeneratorService.GenerateKey().ConfigureAwait(false)
        };
        return Ok(request);
    }

    [HttpPost]
    [Route("[action]")]
    [EnableRateLimiting("UploadLimiter")]
    public async ValueTask<IActionResult> Results([FromBody] JellybenchResult result)
    {
        if (await _requestKeyGeneratorService.ValidateKey(result.DataRequestKey).ConfigureAwait(false) == false)
        {
            return Unauthorized();
        }
        await using var jellybenchDataContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        if (await jellybenchDataContext.BenchRuns.AnyAsync(e => e.DataRequestKey == result.DataRequestKey)
                .ConfigureAwait(false))
        {
            return Conflict();
        }

        var entity = _mapper.Map<BenchRun>(result);
        jellybenchDataContext.Add(entity);
        await jellybenchDataContext.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }
}