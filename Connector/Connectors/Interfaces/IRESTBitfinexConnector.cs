using Connector.Models;

namespace Connector.Connectors.Interfaces
{
    //ITestConnector has been split into two interfaces to follow SOLID principles
    public interface IRESTBitfinexConnector
    {
        public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);

        //period type has been changed to string. BitfinexApi accepts period as a String value("1m","5m" etc.)
        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        public Task<Ticker> GetTickerInfo(string pair);
    }
}
