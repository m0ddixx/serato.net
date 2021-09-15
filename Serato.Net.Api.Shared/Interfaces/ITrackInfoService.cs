using System.Collections.Generic;
using System.Threading.Tasks;
using Serato.Net.Structs;

namespace Serato.Net.Api.Shared.Interfaces
{
    public interface ITrackInfoService
    {
        public Task AddTrackInfosAsync(IEnumerable<TrackInfo> trackInfos);
        public Task SaveChangesAsnyc();
    }
}