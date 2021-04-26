using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThatsLife.Models;

namespace ThatsLife.Views.Shared.Components
{
    /// <summary>
    /// This component provide stock ticker information of top tech stocks
    /// using IExloud's API and deserializes it to the StockQuote Model
    /// </summary>
    public class TickerViewComponent: ViewComponent
    {
        private string _URL;
        private string _IExcloudKey;

        public IConfiguration Configuration { get; }
        public TickerViewComponent(IConfiguration configuration)
        {
            Configuration = configuration;
            _IExcloudKey = Configuration["APIs:IExapix"];
            _URL = $"https://cloud.iexapis.com/stable/stock/market/batch/?symbols=,googl,aapl,fb,msft,amzn,nflx,tsla,intc&types=quote&token={_IExcloudKey}";
        }
        /// <summary>
        /// Gets Stock information from IExcloud API in form of json object and then
        /// deserializes it into a list of StockQuotes.
        /// </summary>
        /// <returns>List of StockQuotes to the Ticker ViewComponent</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<StockQuote> stocks = new List<StockQuote>();
            var json = await ApiHub.GetJson(_URL);
            var root = JObject.Parse(json);
            foreach (var r in root)
            {
                var sybmol = JsonConvert.DeserializeObject<StockQuote>(r.Value["quote"].ToString());
                stocks.Add(sybmol);
            }
            return View("Default", stocks);
        }
    }
}
