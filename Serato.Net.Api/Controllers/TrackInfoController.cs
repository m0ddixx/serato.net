using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serato.Net.Api.Shared;
using Serato.Net.Api.Shared.Interfaces;
using Serato.Net.Api.Shared.Models;
using Serato.Net.Api.Shared.Services;
using Serato.Net.Structs;

namespace Serato.Net.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackInfoController : ControllerBase
    {
        private readonly ITrackInfoService _service;

        public TrackInfoController(ITrackInfoService service)
        {
            _service = service;
        }

        [Consumes("application/json", "text/json")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK | StatusCodes.Status400BadRequest)]
        public async Task SendAggregatedTrackInfos([FromBody] IEnumerable<TrackInfo> trackInfos)
        {
            await _service.AddTrackInfosAsync(trackInfos);
            await _service.SaveChangesAsnyc();
        }
    }
}