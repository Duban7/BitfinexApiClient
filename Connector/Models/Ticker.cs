
namespace Connector.Models
{
    public class Ticker
    {
        public decimal BID { get; set; }
        public decimal BIDsize { get; set; }
        public decimal Ask { get; set; }
        public decimal AskSize { get; set; }
        public decimal DialyChange { get; set; }
        public decimal DailyChangeRelative { get; set; }
        public decimal LastPrice { get; set; }
        public decimal Volume { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public string Pair { get; set; }

    }
}
