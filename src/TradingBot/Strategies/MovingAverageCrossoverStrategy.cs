using TradingBot.Services;
using TradingBot.Models;

namespace TradingBot.Strategies
{
    public class MovingAverageCrossoverStrategy : IStrategy
    {
        private readonly IExchangeAdapter _exchange;
        private readonly List<decimal> _prices = new();
        private readonly int _short = 5;
        private readonly int _long = 20;
        private bool _positionOpen = false;

        public MovingAverageCrossoverStrategy(IExchangeAdapter exchange)
        {
            _exchange = exchange;
        }

        public async Task OnTickAsync(CancellationToken ct = default)
        {
            var price = await _exchange.GetLatestPriceAsync("BTCUSD", ct);
            _prices.Add(price);
            if (_prices.Count > _long) _prices.RemoveAt(0);

            if (_prices.Count < _long) return;

            var shortMa = _prices.TakeLast(_short).Average();
            var longMa = _prices.Average();

            if (!_positionOpen && shortMa > longMa)
            {
                await _exchange.PlaceOrderAsync(new Order { Symbol = "BTCUSD", Side = OrderSide.Buy, Price = price, Quantity = 0.001m }, ct);
                _positionOpen = true;
            }
            else if (_positionOpen && shortMa < longMa)
            {
                await _exchange.PlaceOrderAsync(new Order { Symbol = "BTCUSD", Side = OrderSide.Sell, Price = price, Quantity = 0.001m }, ct);
                _positionOpen = false;
            }
        }
    }
}