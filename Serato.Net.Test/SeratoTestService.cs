using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serato.Net.Services;

namespace Serato.Net.Test
{
    internal class SeratoTestService : IHostedService
    {
        private readonly ILogger<SeratoTestService> _logger;

        public SeratoTestService(ILogger<SeratoTestService> logger, FileWatcher fileWatcher, SessionFileParser sessionFileParser)
        {
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Test Scanner started.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Test Scanner stopped.");
            return Task.CompletedTask;
        }
    }
}
