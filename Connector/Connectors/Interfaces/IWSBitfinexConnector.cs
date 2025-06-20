using Connector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Connectors.Interfaces
{
    public interface IWSBitfinexConnector
    {
        public event Action<Trade> NewBuyTrade;
        public event Action<Trade> NewSellTrade;
        public void SubscribeTrades(string pair, int maxCount = 100);
        public void UnsubscribeTrades(string pair);

        public event Action<Candle> CandleSeriesProcessing;
        public void SubscribeCandles(string pair, int periodInSec, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        public void UnsubscribeCandles(string pair);

    }
}
