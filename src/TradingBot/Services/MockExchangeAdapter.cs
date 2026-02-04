using TradingBot.Models;

namespace TradingBot.Services
{
    public class MockExchangeAdapter : IExchangeAdapter
    {
        private readonly Random _rand = new();

        public Task<decimal> GetLatestPriceAsync(string symbol, CancellationToken ct = default)
        {
            // simple random walk around 100
            var price = 100m + (decimal)(_rand.NextDouble() - 0.5) * 2m;
            return Task.FromResult(decimal.Round(price, 2));
        }

        public Task PlaceOrderAsync(Order order, CancellationToken ct = default)
        {
            // just log to console for now
            Console.WriteLine($"[MockExchange] Placing order: {order.Side} {order.Quantity} {order.Symbol} at {order.Price}");
            return Task.CompletedTask;
        }
    }
}