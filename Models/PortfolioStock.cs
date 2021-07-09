using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class PortfolioStock
    {
        public int PlayerStockId { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PercentChange { get; set; }
        public decimal Change { get; set; }
        public int Shares { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalCost { get { return PurchasePrice * Shares; } set { } }
        public decimal MarketValue { get { return CurrentPrice * Shares; } set { } }

        
        public decimal TodaysGain 
        { 
            get 
            { 
                if(PurchasePrice == CurrentPrice)
                {
                    return 0;
                }
                else
                {
                    return Change * Shares;
                }
                
            } 
            set 
            { }
        }
        public decimal Gain { get { return decimal.Subtract(MarketValue, TotalCost); } set { } }
        public decimal Return { get { return TotalCost == 0 ? 0 : Gain / TotalCost; } set { } }

    }
}
