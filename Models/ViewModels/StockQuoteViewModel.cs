using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.ViewModels
{
    public class StockQuoteViewModel
    {
        
        public PlayerProfile Player { get; set; }
        public string LogoUrl { get; set; }
        public StockQuote StockQuote {get;set;}
        public Company Company { get; set; }
        public int Shares { get; set; }

    }
}
