using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TradingBot.Models;

namespace TradingBot.Services
{
    public class BinancePaperAdapter : IExchangeAdapter
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _secret;
        private readonly string _baseUrl;

        public BinancePaperAdapter()
        {
            _apiKey = Environment.GetEnvironmentVariable("BINANCE_API_KEY") ?? string.Empty;
            _secret = Environment.GetEnvironmentVariable("BINANCE_SECRET_KEY") ?? string.Empty;
            _baseUrl = Environment.GetEnvironmentVariable("BINANCE_BASE_URL") ?? "https://testnet.binance.vision/api";

            _http = new HttpClient { BaseAddress = new Uri(_baseUrl) };
            if (!string.IsNullOrEmpty(_apiKey)) _http.DefaultRequestHeaders.Add("X-MBX-APIKEY", _apiKey);
        }

        private static string ToBinanceSymbol(string symbol)
        {
            if (symbol.EndsWith("USD", StringComparison.OrdinalIgnoreCase))
                return symbol[..^3] + "USDT";
            return symbol;
        }

        public async Task<decimal> GetLatestPriceAsync(string symbol, CancellationToken ct = default)
        {
            var bsymbol = ToBinanceSymbol(symbol);
            var url = $"/api/v3/ticker/price?symbol={bsymbol}";

            var resp = await _http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            if (doc.RootElement.TryGetProperty("price", out var priceEl) && decimal.TryParse(priceEl.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                return price;
            }

            throw new InvalidOperationException("Unable to parse price from Binance response");
        }

        public async Task PlaceOrderAsync(Order order, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_secret))
            {
                Console.WriteLine("BINANCE_API_KEY or BINANCE_SECRET_KEY not set - skipping real order (paper mode). Log only.");
                Console.WriteLine($"[Paper] {order.Side} {order.Quantity} {order.Symbol} @ {order.Price}");
                return;
            }

            var bsymbol = ToBinanceSymbol(order.Symbol);
            var side = order.Side == OrderSide.Buy ? "BUY" : "SELL";
            var qty = order.Quantity.ToString(CultureInfo.InvariantCulture);

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var parameters = new Dictionary<string, string>
            {
                { "symbol", bsymbol },
                { "side", side },
                { "type", "MARKET" },
                { "quantity", qty },
                { "timestamp", timestamp.ToString() }
            };

            var query = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            var signature = Sign(query, _secret);
            var body = query + "&signature=" + signature;

            var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _http.PostAsync("/api/v3/order", content, ct);

            var text = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"Binance order failed: {resp.StatusCode} - {text}");
                return;
            }

            Console.WriteLine($"[Binance Testnet] Order placed: {order.Side} {order.Quantity} {order.Symbol} - Response: {text}");
        }

        private static string Sign(string message, string secret)
        {
            var key = Encoding.UTF8.GetBytes(secret);
            var msg = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(key);
            var hash = hmac.ComputeHash(msg);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
