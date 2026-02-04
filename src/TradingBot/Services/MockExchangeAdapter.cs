using TradingBot.Models;

namespace TradingBot.Services
{
    public class MockExchangeAdapter : IExchangeAdapter
    {
        private readonly Random _rand = new Random();
        private decimal _price = 50000m;

        public Task<decimal> GetLatestPriceAsync(string symbol, CancellationToken ct = default)
        {
            // Simple price simulation with random walk
            _price = _price * (1 + (decimal)(_rand.NextDouble() - 0.5) * 0.01m);
            return Task.FromResult(decimal.Round(_price, 2));
        }

        public Task PlaceOrderAsync(Order order, CancellationToken ct = default)
        {
            Console.WriteLine($"[Paper Trading] {order.Side} {order.Quantity} {order.Symbol} @ ${order.Price:F2}");
            return Task.CompletedTask;
        }
    }
}
