using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;
using ThatsLife.Models.ViewModels;

namespace ThatsLife.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _ProfileRepository;
        private readonly IRepository<PlayerStock> _stockRepository;
        private readonly IRepository<PlayerTransaction> _transactionRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigescoreRepository;
        private readonly IRepository<PlayerCash> _cashRepository;
        private readonly IConfiguration _configuration;
        private string _url;
        private string _iexcloudKey;

        public StockController(SignInManager<IdentityUser> signInManager,
                       UserManager<IdentityUser> userManager,
                       IRepository<PlayerProfile> profileRepository,
                       IRepository<PlayerStock> stockRepository,
                       IRepository<PlayerTransaction> transactionRepository,
                       IRepository<PlayerPrestigeScore> prestigescoreRepository,
                       IRepository<PlayerCash> cashRepository,
                       IConfiguration configuration)
        {
            _userManager = userManager;
            _ProfileRepository = profileRepository;
            _stockRepository = stockRepository;
            _transactionRepository = transactionRepository;
            _prestigescoreRepository = prestigescoreRepository;
            _cashRepository = cashRepository;
            _configuration = configuration;
            _iexcloudKey = _configuration["APIs:IExapix"];
        }


        public async Task<IActionResult> StockQuoteAsync(string symbol)
        {

            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).First();

            StockQuoteViewModel stockQuoteViewModel = new StockQuoteViewModel();
            stockQuoteViewModel.Player = player;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            _url = $"https://cloud.iexapis.com/stable/stock/{symbol}/quote?token={_iexcloudKey}";
            var json = await ApiHub.GetJson(_url);
            StockQuote stockQuote = JsonConvert.DeserializeObject<StockQuote>(json,settings);
            stockQuoteViewModel.StockQuote = stockQuote;

            _url = $"https://cloud.iexapis.com/stable/stock/{symbol}/logo?token={_iexcloudKey}";
            json = await ApiHub.GetJson(_url);
            JObject obj = JObject.Parse(json);
            string logoUrl = (string)obj["url"];
            stockQuoteViewModel.LogoUrl = logoUrl;

            _url = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={_iexcloudKey}";
            json = await ApiHub.GetJson(_url);
            stockQuoteViewModel.Company = JsonConvert.DeserializeObject<Company>(json,settings);

            return View("StockQuote", stockQuoteViewModel);
        }

        /// <summary>
        /// Once Order has been confirmed this endpoint will handle the transaction of
        /// updating players balance and add to player's stocks.  Prestige score
        /// is calculted with the weighted value coming from the P/E Ratio.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="playerId"></param>
        /// <param name="price"></param>
        /// <param name="shares"></param>
        /// <param name="peRatio"></param>
        /// <returns></returns>
        public IActionResult Buy(string symbol, string price, string shares, string peRatio)
        {

            decimal _price = decimal.Parse(price);
            int _shares = int.Parse(shares);
            decimal _peRatio = decimal.Parse(peRatio?? "0");
            decimal total = _price * _shares;

            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).First();

            PlayerCash playerCash = new PlayerCash();
            playerCash.ProfileId = player.Id;
            playerCash.Balance = _cashRepository
               .FindByCondition(p => p.ProfileId == player.Id)
               .OrderByDescending(c => c.CreatedDate)
               .Select(p => p.Balance)
               .FirstOrDefault();

            //verfiy player has enough currency to purchase order
            if (total > playerCash.Balance)
            {
                return View("InsufficentFundsView", total);
            }
            else
            {
                PlayerStock PlayerStock = new PlayerStock();
                PlayerStock.StockSymbol = symbol;
                PlayerStock.ProfileId = player.Id;
                PlayerStock.PurchasePrice = _price;
                PlayerStock.Shares = _shares;
                _stockRepository.Create(PlayerStock);

                playerCash.Balance -= total;
                _cashRepository.Create(playerCash);

                PlayerTransaction playerTransaction = new PlayerTransaction();
                playerTransaction.ProfileId = player.Id;
                playerTransaction.TransactionType = "Buy";
                playerTransaction.TransactionDescription = $"{symbol} Purchased";
                playerTransaction.Quantity = _shares;
                playerTransaction.Price = total;
                _transactionRepository.Create(playerTransaction);

                //prestige score is based on P/E Ratio. 
                PlayerPrestigeScore playerPrestige = new PlayerPrestigeScore();
                playerPrestige.ProfileId = player.Id;
                playerPrestige.Source = "Risk Reward";
                playerPrestige.Score = _prestigescoreRepository
                .FindByCondition(p => p.ProfileId == player.Id)
                .OrderByDescending(c => c.CreatedDate)
                .Select(p => p.Score)
                .FirstOrDefault();
                decimal purchaseToPlayerBalanceRatio = total / playerCash.Balance;  //how vested the player is in this stock
                decimal stockValue = 1000 - _peRatio;  //lower P/E Ratio means stock is more valuable
                playerPrestige.Score += (int)(stockValue * purchaseToPlayerBalanceRatio);
                playerPrestige.PointsEarned = (int)(stockValue * purchaseToPlayerBalanceRatio);
                _prestigescoreRepository.Create(playerPrestige);
            }

           
            
            return RedirectToAction("Portfolio", "stock");
        }

        public IActionResult Sell(string stockId, string value)
        {
            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).FirstOrDefault();


            PlayerStock playerStock = _stockRepository.FindByCondition(s => s.Id == int.Parse(stockId)).FirstOrDefault();
            _stockRepository.Delete(playerStock);


            PlayerCash playerCash = new PlayerCash();
            playerCash.ProfileId = player.Id;
            decimal marketValue = decimal.Parse(value);
            playerCash.Balance = _cashRepository
               .FindByCondition(p => p.ProfileId == player.Id)
               .OrderByDescending(c => c.CreatedDate)
               .Select(p => p.Balance)
               .FirstOrDefault();
            playerCash.Balance += marketValue;
            _cashRepository.Create(playerCash);


            string symbol = playerStock.StockSymbol;
            int shares = playerStock.Shares;
            decimal totalCost = (shares * playerStock.PurchasePrice);

            PlayerTransaction playerTransaction = new PlayerTransaction();
            playerTransaction.ProfileId = player.Id;
            playerTransaction.TransactionType = "Sell";
            playerTransaction.TransactionDescription = $"{symbol} Purchased";
            playerTransaction.Quantity = shares;
            playerTransaction.Price = marketValue;
            _transactionRepository.Create(playerTransaction);


            PlayerPrestigeScore playerPrestige = new PlayerPrestigeScore();
            playerPrestige.ProfileId = player.Id;
            playerPrestige.Source = "Profit Reward";
            decimal stockReturn = marketValue - totalCost;
            playerPrestige.Score = _prestigescoreRepository
                .FindByCondition(p => p.ProfileId == player.Id)
                .OrderByDescending(c => c.CreatedDate)
                .Select(p => p.Score)
                .FirstOrDefault();
            if (stockReturn > 0)
            {
                playerPrestige.Score += (int)stockReturn;
                playerPrestige.PointsEarned = (int)stockReturn;
            }
            _prestigescoreRepository.Create(playerPrestige);


           
            _stockRepository.Delete(playerStock);
 
            return RedirectToAction("Portfolio");
        }

        public async Task<IActionResult> PortfolioAsync()
        {
            var userId = _userManager.GetUserId(User);

            //Retrieves data from database
            StockPortfolioViewModel stockPortfolioViewModel = new StockPortfolioViewModel();
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).First();
            stockPortfolioViewModel.PlayProfile = player;
            IEnumerable<PlayerStock> playerStocks = _stockRepository.FindByCondition(p => p.ProfileId == player.Id);

            if (playerStocks.Any())
            {
                //Builds API Url from list of stocks the player owns
                StringBuilder sb = new StringBuilder();
                sb.Append($@"https://cloud.iexapis.com/stable/stock/market/batch/?types=quote&token={_iexcloudKey}&symbols=");


                for (int i = 0; i < playerStocks.Count(); i++)
                {
                    if (i == playerStocks.Count() - 1)
                    {
                        sb.Append(playerStocks.ElementAt(i).StockSymbol);
                    }
                    else
                    {
                        sb.Append(playerStocks.ElementAt(i).StockSymbol + ",");
                    }
                }
                _url = sb.ToString();




                //Gets real time stock quote for each stock player owns 
                var json = await ApiHub.GetJson(_url);
                var root = JObject.Parse(json);
                List<StockQuote> stocksApi = new List<StockQuote>();
                foreach (var r in root)
                {
                    var stock = JsonConvert.DeserializeObject<StockQuote>(r.Value["quote"].ToString());
                    stocksApi.Add(stock);
                }


                List<PortfolioStock> stocks = new List<PortfolioStock>();
                foreach (var pStocks in playerStocks)
                {
                    StockQuote stock = stocksApi.Where(s => s.Symbol == pStocks.StockSymbol).FirstOrDefault();
                    var portfolioStock = new PortfolioStock();
                    portfolioStock.PlayerStockId = pStocks.Id;
                    portfolioStock.Symbol = stock.Symbol;
                    portfolioStock.Name = stock.CompanyName;
                    portfolioStock.CurrentPrice = stock.LatestPrice;
                    portfolioStock.Change = (decimal)stock.Change;
                    portfolioStock.PercentChange = stock.ChangePercent;
                    portfolioStock.Shares = pStocks.Shares;
                    portfolioStock.PurchasePrice = pStocks.PurchasePrice;
                    portfolioStock.PurchaseDate = pStocks.CreatedDate;
                    stocks.Add(portfolioStock);
                }

                foreach (var stock in stocks)
                {
                    stockPortfolioViewModel.TotalStockValue += stock.MarketValue;
                    stockPortfolioViewModel.TotalStockCost += stock.TotalCost;
                    stockPortfolioViewModel.TotalTodaysGain += stock.TodaysGain;
                }
                stockPortfolioViewModel.Stocks = stocks;
                
            }

            return View("Portfolio", stockPortfolioViewModel);
        }



        public IActionResult Exchange()
        {
            return View();

        }


    }
}
