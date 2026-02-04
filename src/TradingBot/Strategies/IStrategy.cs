namespace TradingBot.Strategies
{
    public interface IStrategy
    {
        Task OnTickAsync(CancellationToken ct = default);
    }
}