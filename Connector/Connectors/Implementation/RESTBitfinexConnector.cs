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
        public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0)
        {
            var timeFrame = GetTimeFrameString(periodInSec);
            string url = $"candles/trade:{timeFrame}:t{pair}/hist";

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
                Time = DateTimeOffset.FromUnixTimeSeconds((long)t[1]),
                Amount = Math.Abs(t[2]),
                Side = t[2] > 0 ? "Buy" : "Sell",
                Price = t[3],
                Pair = pair
            });
        }

        public async Task<object> GetTickerInfo(string pair)
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

        private static string GetTimeFrameString(int periodInSec) => periodInSec switch
        {
            60 => "1m",
            300 => "5m",
            900 => "15m",
            1800 => "30m",
            3600 => "1h",
            10800 => "3h",
            21600 => "6h",
            43200 => "12h",
            86400 => "1D",
            604800 => "1W",
            _ => "1h"
        };

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
