namespace TradingBot.Services
{
    public interface IPortfolioTracker
    {
        void RecordTrade(decimal price, string side);
        Task DisplayStatsAsync();
    }
}
