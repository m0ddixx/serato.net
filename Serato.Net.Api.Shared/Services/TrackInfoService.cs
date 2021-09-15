using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serato.Net.Api.Shared.Interfaces;
using Serato.Net.Api.Shared.Models;
using Serato.Net.Structs;

namespace Serato.Net.Api.Shared.Services
{
    public class TrackInfoService : ITrackInfoService
    {
        private readonly SeratoContext _ctx;

        public TrackInfoService(SeratoContext ctx)
        {
            _ctx = ctx;
        }
        public async Task AddTrackInfosAsync(IEnumerable<TrackInfo> trackInfos)
        {

            var trackInfoList = trackInfos.Select(ti => new TrackInfoEntity() { TrackInfo = ti }).ToList();
            var last = await _ctx.TrackInfos.OrderBy(t => t.TrackInfo.UpdatedAt).LastOrDefaultAsync();
            if (last != default)
                trackInfoList = trackInfoList.Where(t => t.TrackInfo.UpdatedAt >= last.TrackInfo.UpdatedAt).ToList();
            await _ctx.TrackInfos.AddRangeAsync(trackInfoList);
        }

        public Task SaveChangesAsnyc()
        {
            return _ctx.SaveChangesAsync();
        }
    }
}