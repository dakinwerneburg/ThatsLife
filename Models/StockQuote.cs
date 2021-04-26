

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace ThatsLife.Models
{
    public class StockQuote
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public string CalculationPrice { get; set; }
        public float? Open { get; set; }
        public float? High { get; set; }
        public float? Low { get; set; }

        [DisplayFormat(DataFormatString = "c")]
        public decimal LatestPrice { get; set; }

        public long LatestUpdate { get; set; }
        public float? LatestVolume { get; set; }
        public float PreviousClose { get; set; }
        public int PreviousVolume { get; set; }

        public float Change { get; set; }

        public decimal ChangePercent { get; set; }
        public int AvgTotalVolume { get; set; }
        public long MarketCap { get; set; }
        public float PeRatio { get; set; }
        public float Week52High { get; set; }
        public float Week52Low { get; set; }
        public float YtdChange { get; set; }
        public bool IsUSMarketOpen { get; set; }
        public DateTime LatesUpdateDT
        {
            get
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(LatestUpdate).LocalDateTime;
            }
        }

    }
}
