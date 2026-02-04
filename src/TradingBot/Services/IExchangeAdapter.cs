using TradingBot.Models;

namespace TradingBot.Services
{
    public interface IExchangeAdapter
    {
        Task<decimal> GetLatestPriceAsync(string symbol, CancellationToken ct = default);
        Task PlaceOrderAsync(Order order, CancellationToken ct = default);
    }
}