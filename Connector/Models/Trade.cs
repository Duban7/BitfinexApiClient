namespace Connector.Models
{
    public class Trade
    {
        /// <summary>
        /// Валютная пара
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Цена трейда
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Объем трейда
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Направление (buy/sell)
        /// </summary>
        public string Side { get; set; }

        /// <summary>
        /// Время трейда
        /// </summary>
        public DateTimeOffset Time { get; set; }


        /// <summary>
        /// Id трейда
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Trade State
        /// </summary>
        // Added trade state. Websocket updates data and shows update with parameters "te" or "tu". So to separate already executed and updated trades added that parameter.
        public bool IsExecuted { get; set; }

    }
}
