using Microsoft.Extensions.Hosting;
using TradingBot.Strategies;

namespace TradingBot.Services
{
    public class TradingHostedService : BackgroundService
    {
        private readonly IStrategy _strategy;
        private readonly IPortfolioTracker _portfolioTracker;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
        private int _tickCount = 0;

        public TradingHostedService(IStrategy strategy, IPortfolioTracker portfolioTracker)
        {
            _strategy = strategy;
            _portfolioTracker = portfolioTracker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _strategy.OnTickAsync(stoppingToken);
                
                // Display portfolio stats every 30 seconds
                _tickCount++;
                if (_tickCount % 30 == 0)
                {
                    await _portfolioTracker.DisplayStatsAsync();
                }
                
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}