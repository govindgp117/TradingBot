using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradingBot.Strategies;
using TradingBot.Services;
using TradingBot.Models;
using Xunit;

namespace TradingBot.Tests
{
    public class MovingAverageTests
    {
        [Fact]
        public void TestAverageCalculation()
        {
            var prices = new List<decimal> { 1, 2, 3, 4, 5 };
            var avg = prices.Average();
            Assert.Equal(3m, avg);
        }

        [Fact]
        public async Task StrategyPlacesOrderOnCrossover()
        {
            // Use the real strategy but inject a fake exchange to capture orders
            var fake = new FakeExchangeAdapter(new[] { 100m, 101m, 102m, 103m, 104m, 105m, 106m, 107m, 108m, 109m, 110m, 111m, 112m, 113m, 114m, 115m, 116m, 117m, 118m, 119m });
            var strat = new MovingAverageCrossoverStrategy(fake);

            for (int i = 0; i < 25; i++)
            {
                await strat.OnTickAsync(CancellationToken.None);
            }

            Assert.True(fake.Orders.Any());
        }

        private class FakeExchangeAdapter : IExchangeAdapter
        {
            private readonly decimal[] _prices;
            private int _i = 0;
            public List<Order> Orders { get; } = new();

            public FakeExchangeAdapter(decimal[] prices) => _prices = prices;

            public Task<decimal> GetLatestPriceAsync(string symbol, CancellationToken ct = default)
            {
                var p = _prices[_i % _prices.Length];
                _i++;
                return Task.FromResult(p);
            }

            public Task PlaceOrderAsync(Order order, CancellationToken ct = default)
            {
                Orders.Add(order);
                return Task.CompletedTask;
            }
        }
    }
}
