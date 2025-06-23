using Connector.Connectors.Implementation;
using Connector.Models;

namespace Tests
{

    public class WSConnectorTests : IDisposable
    {
        private readonly WSBitfinexConnector _connector;
        private readonly List<Trade> _receivedTrades = new List<Trade>();
        private readonly List<Candle> _receivedCandles = new List<Candle>();
        private readonly string _testPair = "BTCUSD";

        public WSConnectorTests()
        {
            _connector = new WSBitfinexConnector();
            _connector.NewBuyTrade += trade => _receivedTrades.Add(trade);
            _connector.NewSellTrade += trade => _receivedTrades.Add(trade);
            _connector.CandleSeriesProcessing += candle => _receivedCandles.Add(candle);
        }

        [Fact]
        public void SubscribeTradesTest()
        {
            // Arrange
            var resetEvent = new AutoResetEvent(false);
            _connector.NewBuyTrade += _ => resetEvent.Set();
            _connector.NewSellTrade += _ => resetEvent.Set();

            // Act
            _connector.SubscribeTrades(_testPair);
            bool eventReceived = resetEvent.WaitOne(TimeSpan.FromSeconds(10));
            Thread.Sleep(1000);

            // Assert
            Assert.True(eventReceived, "Не получены события о трейдах");
            Assert.NotEmpty(_receivedTrades);
        }

        [Fact]
        public void SubscribeCandlesTest()
        {
            // Arrange
            var resetEvent = new AutoResetEvent(false);
            _connector.CandleSeriesProcessing += _ => resetEvent.Set();

            // Act
            _connector.SubscribeCandles(_testPair, "1m");
            bool eventReceived = resetEvent.WaitOne(TimeSpan.FromSeconds(10));
            Thread.Sleep(1000);

            // Assert
            Assert.True(eventReceived, "Не получены события о свечах");
            Assert.NotEmpty(_receivedCandles);
        }

        [Fact]
        public void UnsubscribeTradesTest()
        {
            // Arrange
            _connector.SubscribeTrades(_testPair);
            Thread.Sleep(3000);
            int initialCount = _receivedTrades.Count;

            // Act
            _connector.UnsubscribeTrades(_testPair);
            int countAfterUnsubscribe = _receivedTrades.Count;
            Thread.Sleep(2000);

            // Assert
            Assert.True(countAfterUnsubscribe == initialCount);
            Assert.Equal(countAfterUnsubscribe, _receivedTrades.Count);
        }

        [Fact]
        public void UnsubscribeCandlesTest()
        {
            // Arrange
            _connector.SubscribeCandles(_testPair, "1m");
            Thread.Sleep(2000);
            int initialCount = _receivedCandles.Count;

            // Act
            _connector.UnsubscribeCandles(_testPair);
            int countAfterUnsubscribe = _receivedCandles.Count;
            Thread.Sleep(2000);

            // Assert
            Assert.True(countAfterUnsubscribe == initialCount);
            Assert.Equal(countAfterUnsubscribe, _receivedCandles.Count);
        }
        public void Dispose()
        {
            _connector.UnsubscribeTrades(_testPair);
            _connector.UnsubscribeCandles(_testPair);
            _connector.Dispose();
        }
    }
}
