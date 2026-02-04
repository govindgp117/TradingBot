using Microsoft.Extensions.Hosting;
using TradingBot.Strategies;

namespace TradingBot.Services
{
    public class TradingHostedService : BackgroundService
    {
        private readonly IStrategy _strategy;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);

        public TradingHostedService(IStrategy strategy)
        {
            _strategy = strategy;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _strategy.OnTickAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}