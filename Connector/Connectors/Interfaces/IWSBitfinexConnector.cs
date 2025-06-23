using Connector.Models;

namespace Connector.Connectors.Interfaces
{
    //ITestConnector has been split into two interfaces to follow SOLID principles
    public interface IWSBitfinexConnector
    {
        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public void SubscribeTrades(string pair, int maxCount = 100);
        public void UnsubscribeTrades(string pair);

        public event Action<Candle> CandleSeriesProcessing;

        //period type has been changed to string. BitfinexApi accepts period as a String value("1m","5m" etc.)
        public void SubscribeCandles(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        public void UnsubscribeCandles(string pair);

    }
}
