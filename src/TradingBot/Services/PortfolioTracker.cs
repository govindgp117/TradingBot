namespace TradingBot.Services
{
    public class PortfolioTracker : IPortfolioTracker
    {
        private decimal _balance = 100000m;
        private decimal _startingBalance = 100000m;
        private readonly List<(decimal price, string side)> _trades = new();
        private int _winningTrades = 0;

        public void RecordTrade(decimal price, string side)
        {
            _trades.Add((price, side));
            
            // Simple P&L calculation: pairs of buy/sell
            if (_trades.Count % 2 == 0)
            {
                var buyPrice = _trades[_trades.Count - 2].price;
                var sellPrice = _trades[_trades.Count - 1].price;
                var profit = (sellPrice - buyPrice) * 0.001m; // Trading 0.001 quantity
                
                _balance += profit;
                if (profit > 0) _winningTrades++;
            }
        }

        public async Task DisplayStatsAsync()
        {
            var totalPnL = _balance - _startingBalance;
            var pnLPercent = (_balance - _startingBalance) / _startingBalance * 100;

            var completedTrades = _trades.Count / 2; // integer number of completed buy/sell pairs
            var winRate = completedTrades > 0 ? (_winningTrades / (decimal)completedTrades) * 100 : 0;

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("PORTFOLIO STATUS");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Starting Balance:    ${_startingBalance:F2}");
            Console.WriteLine($"Current Balance:     ${_balance:F2}");
            Console.WriteLine($"Total P&L:           ${totalPnL:F2} ({pnLPercent:F2}%)");
            Console.WriteLine($"Total Trades:        {_trades.Count / 2}");
            Console.WriteLine($"Winning Trades:      {_winningTrades}");
            Console.WriteLine($"Win Rate:            {winRate:F2}%");
            Console.WriteLine(new string('=', 60) + "\n");

            await Task.CompletedTask;
        }
    }
}
