using Connector.Connectors.Implementation;

namespace Tests
{
    public class RESTConnectorTests
    {
        private readonly RESTBitfinexConnector _connector = new RESTBitfinexConnector();
        private readonly string _testPair = "BTCUSD";

        [Fact]
        public async Task GetTradesTest()
        {
            // Act
            var trades = await _connector.GetNewTradesAsync(_testPair, 10);

            // Assert
            Assert.NotNull(trades);
            Assert.InRange(trades.Count(), 1, 10);
            Assert.All(trades, t => Assert.NotNull(t.Id));
        }

        [Fact]
        public async Task GetCandlesTest()
        {

            // Act
            var candles = await _connector.GetCandleSeriesAsync(_testPair, "1m");

            // Assert
            Assert.NotNull(candles);
            Assert.NotEmpty(candles);
            Assert.All(candles, c => Assert.True(c.OpenPrice > 0));
        }

        [Fact]
        public async Task GetTickerTest()
        {
            // Act
            var ticker = await _connector.GetTickerInfo(_testPair);

            // Assert
            Assert.NotNull(ticker);
            Assert.True(ticker.LastPrice > 0);
            Assert.True(ticker.BID > 0);
            Assert.True(ticker.Ask > 0);
            Assert.True(ticker.BID <= ticker.Ask);
        }
    }
}
