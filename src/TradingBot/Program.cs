using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingBot;
using TradingBot.Services;
using TradingBot.Strategies;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Use BinancePaperAdapter for paper trading via Binance testnet (set BINANCE_API_KEY/SECRET)
        services.AddSingleton<IExchangeAdapter, BinancePaperAdapter>();
        services.AddSingleton<IStrategy, MovingAverageCrossoverStrategy>();
        services.AddSingleton<IPortfolioTracker, PortfolioTracker>();
        services.AddHostedService<TradingHostedService>();
    })
    .ConfigureLogging(logging => logging.AddConsole())
    .Build();

await host.RunAsync();
