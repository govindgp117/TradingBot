namespace TradingBot.Models
{
    public enum OrderSide { Buy, Sell }

    public class Order
    {
        public string Symbol { get; set; } = string.Empty;
        public OrderSide Side { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}