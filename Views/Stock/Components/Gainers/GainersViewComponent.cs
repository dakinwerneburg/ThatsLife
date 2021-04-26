using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ThatsLife.Models;


namespace ThatsLife.Views.Stock.Components.Gainers
{
    public class GainersViewComponent : ViewComponent
    {
        private string _URL;
        private string _IExcloudKey;

        public IConfiguration Configuration { get; }
        public GainersViewComponent(IConfiguration configuration)
        {
            Configuration = configuration;
            _IExcloudKey = Configuration["APIs:IExapix"];
            _URL = $"https://cloud.iexapis.com/stable/stock/market/list/gainers?token={_IExcloudKey}";
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
                    var root = JArray.Parse(json);
                    var children = root.Children();
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    foreach (JToken t in children)
                    {
                        var symbol = JsonConvert.DeserializeObject<StockQuote>(t.ToString(),settings);
                        stocks.Add(symbol);
                    }
            return View("Default", stocks);
        }
    }
}
