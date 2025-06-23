namespace Connector.Models
{
    public class Candle
    {
        /// <summary>
        /// Валютная пара
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Цена открытия
        /// </summary>
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// Максимальная цена
        /// </summary>
        public decimal HighPrice { get; set; }

        /// <summary>
        /// Минимальная цена
        /// </summary>
        public decimal LowPrice { get; set; }

        /// <summary>
        /// Цена закрытия
        /// </summary>
        public decimal ClosePrice { get; set; }

        // public decimal TotalPrice { get; set; }
        // Bitfinex api doesn't return Total price of candle

        /// <summary>
        /// Partial (Общий объем)
        /// </summary>
        public decimal TotalVolume { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        public DateTimeOffset OpenTime { get; set; }

    }
}
