using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.ViewModels;

namespace ThatsLife.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _ProfileRepository;
        private readonly IRepository<PlayerStock> _StockRepository;
        private readonly IRepository<PlayerTransaction> _TransactionRepository;
        private readonly IConfiguration _Configuration;
        private string _URL;
        private string _IExcloudKey;
        private StockQuote stockQuote;


        public StockController(SignInManager<IdentityUser> signInManager,
                       UserManager<IdentityUser> userManager,
                       IRepository<PlayerProfile> profileRepository,
                       IRepository<PlayerStock> stockRepository,
                       IRepository<PlayerTransaction> transactionRepository,
                       IConfiguration configuration
                       )
        {
            _userManager = userManager;
            _ProfileRepository = profileRepository;
            _StockRepository = stockRepository;
            _TransactionRepository = transactionRepository;
            _Configuration = configuration;
        }


        public async Task<IActionResult> StockQuoteAsync(string symbol)
        {

            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).First();

            StockQuoteViewModel stockQuoteViewModel = new StockQuoteViewModel();
            stockQuoteViewModel.Player = player;

            _IExcloudKey = _Configuration["APIs:IExapix"];
            _URL = $"https://cloud.iexapis.com/stable/stock/{symbol}/quote?token={_IExcloudKey}";
            var json = await ApiHub.GetJson(_URL);
            stockQuote = JsonConvert.DeserializeObject<StockQuote>(json);
            stockQuoteViewModel.StockQuote = stockQuote;

            _URL = $"https://cloud.iexapis.com/stable/stock/{symbol}/logo?token={_IExcloudKey}";
            json = await ApiHub.GetJson(_URL);
            JObject obj = JObject.Parse(json);
            string logoUrl = (string)obj["url"];
            stockQuoteViewModel.LogoUrl = logoUrl;

            _URL = $"https://cloud.iexapis.com/stable/stock/{symbol}/company?token={_IExcloudKey}";
            json = await ApiHub.GetJson(_URL);
            stockQuoteViewModel.Company = JsonConvert.DeserializeObject<Company>(json);

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
        public IActionResult Buy(string symbol, string playerId, string price, string shares, string peRatio)
        {
            PlayerStock PlayerStock = new PlayerStock();
            PlayerTransaction playerTransaction = new PlayerTransaction();
            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.Id == int.Parse(playerId)).First();

            decimal _price = decimal.Parse(price);
            int _shares = int.Parse(shares);
            decimal _peRatio = decimal.Parse(peRatio);
            decimal total = _price * _shares;

            //prestige score is based on P/E Ratio. 
            decimal purchaseToPlayerCurrencyRatio = total / player.Currency;  //how vested the player is in this stock
            decimal stockValue = 100 - _peRatio;  //lower P/E Ratio means stock is more valuable
            decimal PrestigeScore = stockValue * _shares * purchaseToPlayerCurrencyRatio;
            int score = (int)PrestigeScore;

            //verfiy player has enough currency to purchase order
            if (total > player.Currency)
            {
                return View("InsufficentFundsView",total);
            }
            else
            {
                PlayerStock.StockSymbol = symbol;
                PlayerStock.ProfileId = player.Id;
                PlayerStock.PurchasePrice = _price;
                PlayerStock.Shares = _shares;

                playerTransaction.ProfileId = player.Id;
                playerTransaction.TransactionType = "Buy";
                playerTransaction.TransactionDescription = $"Stock Purchase";
                playerTransaction.Quantity = _shares;
                playerTransaction.Price = total;


                playerTransaction.PrestigeScore = score;


                player.PrestigeScore += score;
                player.Currency -= total;

                _StockRepository.Create(PlayerStock);
                _TransactionRepository.Create(playerTransaction);
                _ProfileRepository.Update(player);

            }
            return RedirectToAction("Exchange", "stock");
        }


        public IActionResult Exchange()
        {
            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _ProfileRepository.FindByCondition(p => p.UserId == userId).First();
            return View(player);
        }


    }
}
