using Connector.Connectors.Interfaces;
using Connector.Models;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Connector.Connectors.Implementation
{
    public class WSBitfinexConnector : IWSBitfinexConnector, IDisposable
    {
        private ClientWebSocket _webSocket;
        private readonly Uri _uri = new Uri("wss://api-pub.bitfinex.com/ws/2");
        private readonly Dictionary<string, int> _tradeSubscriptions = new Dictionary<string, int>();
        private readonly Dictionary<string, string> _candleSubscriptions = new Dictionary<string, string>();
        private readonly Dictionary<int, string> _channelToPairMap = new Dictionary<int, string>();

        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public event Action<Candle> CandleSeriesProcessing;

        public WSBitfinexConnector()
        {
            
        }

        private async Task ConnectWebSocketAsync()
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(_uri, CancellationToken.None);
            _ = Task.Run(WebSocketMessageLoop);
        }

        private async Task WebSocketMessageLoop()
        {
            var buffer = new byte[4096];

            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                StringBuilder messageBuilder = new();
                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                while (!result.EndOfMessage) 
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }

                string message = messageBuilder.ToString();
                ProcessWebSocketMessage(message);
            }
        }

        private void ProcessWebSocketMessage(string message)
        {
            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("event", out var eventProp))
                {
                    if (eventProp.GetString() == "subscribed" && root.TryGetProperty("chanId", out var chanId))
                    {
                        var channel = root.GetProperty("channel").GetString();

                        root.TryGetProperty("symbol", out var element);
                        if(element.ValueKind == JsonValueKind.Undefined) 
                            root.TryGetProperty("key", out element);
                        
                        string prop = element.GetString()!.Split(':').Last();

                        _channelToPairMap[chanId.GetInt32()] = $"{channel}:{prop.Replace("t", "")}";
                        Debug.WriteLine("Subscribed ----- "+channel);
                    }
                    return;
                }

                if (root.ValueKind == JsonValueKind.Array)
                {
                    var data = root.EnumerateArray().ToArray();
                    if (!_channelToPairMap.TryGetValue(data[0].GetInt32(), out var pairInfo))
                        return;

                    var channelType = pairInfo.Split(':')[0];
                    var pair = pairInfo.Split(':')[1];

                    if (data[1].ToString() == "hb")
                    {
                        Debug.WriteLine("Heart beat----------");
                        return;
                    }
                    switch (channelType)
                    {
                        case "trades":
                            ProcessTradeUpdate(pair, data);
                            break;
                        case "candles":
                            ProcessCandleUpdate(pair, data);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing WebSocket message: {ex.Message}");
            }
        }

        private async Task SendWebSocketMessage(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
                return;

            var bytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void ProcessTradeUpdate(string pair, JsonElement[] dataArray)
        {
            if (dataArray[1].ValueKind == JsonValueKind.Array)
            {
                var trades = dataArray[1].EnumerateArray().ToArray();
                if (trades.Length > 0)
                {
                    foreach (var trade in trades)
                    {
                        var tradeData = trade.EnumerateArray().ToArray();
                        ProcessSingleTrade(tradeData, pair, "te");
                    }
                }
            }
            else
            {
                    var tradeData = dataArray[2].EnumerateArray().ToArray();
                    ProcessSingleTrade(tradeData, pair, dataArray[1].GetString()!);    
            }
        }

        private void ProcessSingleTrade(JsonElement[] tradeData, string pair, string tradeState)
        {
            if (tradeData.Length >= 4)
            {
                var tradeObj = new Trade
                {
                    Pair = pair,
                    Id = tradeData[0].GetInt64().ToString(),
                    Time = DateTimeOffset.FromUnixTimeMilliseconds(tradeData[1].GetInt64()).DateTime,
                    Amount = Math.Abs(tradeData[2].GetDecimal()),
                    Price = tradeData[3].GetDecimal(),
                    Side = tradeData[2].GetDecimal() > 0 ? "Buy" : "Sell",
                    IsExecuted = tradeState == "te" ? true : false
                };

                if (tradeObj.Side == "Buy")
                    NewBuyTrade?.Invoke(tradeObj);
                else
                    NewSellTrade?.Invoke(tradeObj);
            }
        }

        private void ProcessCandleUpdate(string pair, JsonElement[] dataArray)
        {
            var candles = dataArray[1].EnumerateArray().ToArray();
            if (candles.Length > 6)
            {
                foreach (var data in candles)
                {
                    var candleData = data.EnumerateArray().ToArray();
                    ProcessSingleCandle(candleData, pair);
                }
            }
            else ProcessSingleCandle(candles, pair);
        }

        private void ProcessSingleCandle(JsonElement[] candleData, string pair)
        {
            if (candleData.Length >= 6)
            {
                var candle = new Candle
                {
                    Pair = pair,
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(candleData[0].GetInt64()).DateTime,
                    OpenPrice = candleData[1].GetDecimal(),
                    ClosePrice = candleData[2].GetDecimal(),
                    HighPrice = candleData[3].GetDecimal(),
                    LowPrice = candleData[4].GetDecimal(),
                    TotalVolume = candleData[5].GetDecimal()
                };
                CandleSeriesProcessing?.Invoke(candle);
            }
        }


        public async void SubscribeTrades(string pair, int maxCount = 100)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                await ConnectWebSocketAsync();

            if (_tradeSubscriptions.ContainsKey(pair))
                return;

            var subscribeRequest = new
            {
                @event = "subscribe",
                channel = "trades",
                symbol = $"t{pair}"
            };

            await SendWebSocketMessage(JsonSerializer.Serialize(subscribeRequest));
            _tradeSubscriptions[pair] = maxCount;
        }

        public async void UnsubscribeTrades(string pair)
        {
            if (!_tradeSubscriptions.ContainsKey(pair))
                return;

            var channelId = _channelToPairMap.FirstOrDefault(x => x.Value == $"trades:{pair}").Key;
            if (channelId > 0)
            {
                var unsubscribeRequest = new
                {
                    @event = "unsubscribe",
                    chanId = channelId
                };
                await SendWebSocketMessage(JsonSerializer.Serialize(unsubscribeRequest));
            }

            _tradeSubscriptions.Remove(pair);
        }

        public async void SubscribeCandles(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
                await ConnectWebSocketAsync();

            if (_candleSubscriptions.ContainsKey(pair))
                return;

            if (!AllowedPeriods.Contains(period)) period = "1m";
            var subscribeRequest = new
            {
                @event = "subscribe",
                channel = "candles",
                key = $"trade:{period}:t{pair}"
            };

            await SendWebSocketMessage(JsonSerializer.Serialize(subscribeRequest));
            _candleSubscriptions[pair] = period;
        }

        public async void UnsubscribeCandles(string pair)
        {
            if (!_candleSubscriptions.ContainsKey(pair))
                return;

            var channelId = _channelToPairMap.FirstOrDefault(x => x.Value == $"candles:{pair}").Key;
            if (channelId > 0)
            {
                var unsubscribeRequest = new
                {
                    @event = "unsubscribe",
                    chanId = channelId
                };
                await SendWebSocketMessage(JsonSerializer.Serialize(unsubscribeRequest));
            }

            _candleSubscriptions.Remove(pair);
        }

        private List<string> AllowedPeriods = new List<string> { "1m", "5m", "15m", "30m", "1h", "3h", "6h", "12h", "1D", "1W" };
        public void Dispose()
        {
            _webSocket.Dispose();
        }
    }
}
