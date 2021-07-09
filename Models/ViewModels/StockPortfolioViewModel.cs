using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.ViewModels
{
    public class StockPortfolioViewModel
    {
        public IEnumerable<PortfolioStock> Stocks { get; set; }
        public PlayerProfile PlayProfile { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal TotalStockCost { get; set; }

        public decimal TotalReturn 
        { 
            get 
            { 
                return TotalStockValue - TotalStockCost; 
            }
            set
            {
            }
        }

        public decimal TotalReturnPercentage
        {
            get
            {
                return TotalStockCost == 0 ? 0 : (TotalStockValue - TotalStockCost) / TotalStockCost;
            }
            set
            {
            }
        }

        public decimal TotalTodaysGain { get; set; }

        public decimal TotalTodaysGainPercent
        {
            get
            {
                
                return TotalStockCost == 0 ? 0 : TotalTodaysGain / TotalStockCost;
            }
            set
            {
            }
        }


    }
}
