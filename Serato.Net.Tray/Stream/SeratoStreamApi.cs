using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serato.Net.Services;
using Serato.Net.Util;
using System.Linq;
using Serato.Net.Enum;
using Serato.Net.Extensions;

namespace Serato.Net.Tray.Stream
{
    public class SeratoStreamApi : IHostedService
    {
        private readonly FileWatcher _fileWatcher;
        private readonly SessionFileParser _fileParser;
        private readonly ILogger<SeratoStreamApi> _logger;
        private readonly HttpClient _httpClient;

        public SeratoStreamApi(FileWatcher fileWatcher, SessionFileParser fileParser, ILogger<SeratoStreamApi> logger)
        {
            _fileWatcher = fileWatcher;
            _fileParser = fileParser;
            _logger = logger;

            _httpClient = new HttpClient();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _fileParser.FinishedParsingTracks += async (sender, e) => await FileParserOnFinishedParsingTracks(sender, e, cancellationToken);
            return Task.CompletedTask;
        }

        private async Task FileParserOnFinishedParsingTracks(object sender, FinishedParsingTracksEventArgs e, CancellationToken cancellationToken)
        {
            var list = await e.TrackInfos.Where(t => t.TrackStatus == TrackStatus.Played).ToListAsync(cancellationToken);
            var json = await list.ToJsonAsync(e.Name, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient.Dispose();
            return Task.CompletedTask;
        }
    }
}