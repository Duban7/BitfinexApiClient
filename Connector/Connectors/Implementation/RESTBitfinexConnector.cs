using Connector.Connectors.Interfaces;
using Connector.Models;
using System.Net.Http.Json;

namespace Connector.Connectors.Implementation
{
    public class RESTBitfinexConnector : IRESTBitfinexConnector, IDisposable
    {
        private readonly HttpClient _httpClient;
        public RESTBitfinexConnector()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api-pub.bitfinex.com/v2/");
        }
        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            if(!AllowedPeriods.Contains(period)) period = "1h";
            string url = $"candles/trade:{period}:t{pair}/hist";

            List<string> queryParams = new();

            if (from!=null && from.HasValue)
                queryParams.Add("start=" + from.Value.ToUnixTimeMilliseconds());
            if (to!=null && to.HasValue)
                queryParams.Add("start=" + to.Value.ToUnixTimeMilliseconds());
            if (count!=null && count>0)
                queryParams.Add("limit=" + count.ToString());

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            decimal[][]? candles = await response.Content.ReadFromJsonAsync<decimal[][]>();

            if (candles == null) return [];
            return candles.Select(c => new Candle
            {
                Pair = pair,
                OpenPrice = c[1],
                HighPrice = c[3],
                LowPrice = c[4],
                ClosePrice = c[2],
                TotalVolume = c[5],
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)c[0])
            });
        }

        public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount)
        {
            var response = await _httpClient.GetAsync($"trades/t{pair}/hist?limit={maxCount}");
            response.EnsureSuccessStatusCode();

            decimal[][]? trades = await response.Content.ReadFromJsonAsync<decimal[][]>();
            if (trades == null) return [];
            return trades.Select(t => new Trade
            {
                Id = t[0].ToString(),
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)t[1]),
                Amount = Math.Abs(t[2]),
                Side = t[2] > 0 ? "Buy" : "Sell",
                Price = t[3],
                Pair = pair
            });
        }

        public async Task<Ticker> GetTickerInfo(string pair)
        {
            var response = await _httpClient.GetAsync($"ticker/t{pair}");
            response.EnsureSuccessStatusCode();

            decimal[]? data = await response.Content.ReadFromJsonAsync<decimal[]>();
            Ticker ticker = new()
            {
                Pair = pair,
                BID = data[0],
                BIDsize = data[1],
                Ask = data[2],
                AskSize = data[3],
                DialyChange = data[4],
                DailyChangeRelative = data[5],
                LastPrice = data[6],
                Volume = data[7],
                High = data[8],
                Low = data[9],
            };

            return ticker;
        }

        private List<string> AllowedPeriods = new List<string> {"1m","5m","15m","30m", "1h", "3h", "6h", "12h", "1D","1W"  };

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
