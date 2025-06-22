using Connector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Connectors.Interfaces
{
    public interface IRESTBitfinexConnector
    {
        public Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
        public Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, string period, DateTimeOffset? from = null, DateTimeOffset? to = null, long? count = 0);
        public Task<Ticker> GetTickerInfo(string pair);
    }
}
