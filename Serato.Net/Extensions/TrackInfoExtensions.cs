using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serato.Net.Structs;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Serato.Net.Extensions
{
    public static class TrackInfoExtensions
    {
        public static async Task<string> ToJsonAsync(this IEnumerable<TrackInfo> trackInfos, string sessionName, CancellationToken cancellationToken = default)
        {
            if (trackInfos == null)
                throw new ArgumentNullException(nameof(trackInfos));
            var obj = new
            {
                host = Environment.MachineName,
                name = sessionName,
                trackInfos = trackInfos,
            };
            await using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, obj, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                IncludeFields = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
            }, cancellationToken);
            ms.Position = 0;
            using var reader = new StreamReader(ms);
            return await reader.ReadToEndAsync();
        }
    }
}
