

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;
using ThatsLife.Models.ViewModels;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ThatsLife.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ThatsLife.Controllers
{
    
    [Authorize]
    public class TriviaController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Difficulty> _difficultyRepository;
        private readonly IRepository<QuestionType> _questionTypeRepository;
        private readonly IRepository<PlayerQuiz> _quizRepository;
        private readonly IRepository<PlayerCash> _cashRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigeScoreRepository;
        private readonly IRepository<PlayerTransaction> _transactionRepository;

        public TriviaController(UserManager<IdentityUser> userManager,
            IRepository<PlayerProfile> profileRepository,
        IRepository<Category> categoryRepository,
        IRepository<Difficulty> difficultyRepository,
        IRepository<QuestionType> questionTypeRepository,
        IRepository<PlayerQuiz> quizRepository,
        IRepository<PlayerTransaction> transactionRepository,
        IRepository<PlayerPrestigeScore> prestigeScoreRepository,
        IRepository<PlayerCash> cashRepository
        )
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
            _categoryRepository = categoryRepository;
            _difficultyRepository = difficultyRepository;
            _questionTypeRepository = questionTypeRepository;
            _quizRepository = quizRepository;
            _transactionRepository = transactionRepository;
            _cashRepository = cashRepository;
            _prestigeScoreRepository = prestigeScoreRepository;
        }

    
        
        public IActionResult Parameters()
        {
            TriviaParametersViewModel triviaParametersViewModel = new TriviaParametersViewModel();


            triviaParametersViewModel.Categories = _categoryRepository.FindAll().ToList().Select(c => new SelectListItem
            {
                Value = c.TriviaDbCategoryId.ToString(),
                Text = c.Name
            });
            triviaParametersViewModel.Difficulies = _difficultyRepository.FindAll().ToList().Select(c => new SelectListItem
            {
                Value = c.Name.ToLower(),
                Text = c.Name
            }) ;
            triviaParametersViewModel.TypeList = _questionTypeRepository.FindAll().ToList().Select(c => new SelectListItem
            {
                Value = c.Name == "Multiple Choice"? "multiple" : "boolean",
                Text = c.Name
            });

            return View(triviaParametersViewModel);
        }

         public async Task<IActionResult> QuestionsAsync(TriviaParametersViewModel triviaParameters)
        {

            int categoryId = triviaParameters.CategoryId;
            string difficulty = triviaParameters.Difficulty;
            string questionType = triviaParameters.QuestionType;
            int amount = triviaParameters.NumberOfQuestions;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            string sessionToken = HttpContext.Session.GetString("ApiSessionToken");
            if ( sessionToken == null)
            {
                string tokenRequestUrl = @"https://opentdb.com/api_token.php?command=request";
                var jsonResponse = await ApiHub.GetJson(tokenRequestUrl);
                var resultJToken = JsonConvert.DeserializeObject<JToken>(jsonResponse);
                var apiSessionToken = resultJToken["token"].ToString();
                HttpContext.Session.SetString("ApiSessionToken", apiSessionToken);
                sessionToken = apiSessionToken;


            }
            string url = $@"https://opentdb.com/api.php?amount={amount}&category={categoryId}&difficulty={difficulty}&type={questionType}&token={sessionToken}";
            var json = await ApiHub.GetJson(url);

            var result = JsonConvert.DeserializeObject<Questions>(json);
            TriviaQuestionsViewModel triviaQuestionsViewModel = new TriviaQuestionsViewModel();
            triviaQuestionsViewModel.Questions = result;

            return View("questions",triviaQuestionsViewModel);
        }

        public IActionResult Performance()
        {
            var triviaPerformanceViewModel = new TriviaPerformanceViewModel();
            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _profileRepository.FindByCondition(p => p.UserId == userId).First();
            triviaPerformanceViewModel.Quizes = _quizRepository.FindByCondition(p => p.Player == player);
            int totalCorrect = triviaPerformanceViewModel.Quizes.Select(q => q.Correct).Sum();
            int totalAttempted = triviaPerformanceViewModel.Quizes.Select(q => q.Attempted).Sum();
            triviaPerformanceViewModel.TotalAttempted = totalAttempted;
            triviaPerformanceViewModel.TotalCorrect = totalCorrect;
            triviaPerformanceViewModel.TotalIncorrect = totalAttempted - totalCorrect;

            return View(triviaPerformanceViewModel);
        }

        public IActionResult Results(TriviaQuestionsViewModel triviaQuestionsViewModel)
        {
            TriviaQuestionsViewModel t = triviaQuestionsViewModel;
            string difficultySelect = t.Questions.Results.First().Difficulty;
            string categorySelect = t.Questions.Results.First().Category;
            string typeSelect = t.Questions.Results.First().Type == "multiple" ? "Multiple Choice" : "True/False";


            var userId = _userManager.GetUserId(User);
            PlayerProfile player = _profileRepository.FindByCondition(p => p.UserId == userId).First();
            Difficulty difficulty = _difficultyRepository.FindByCondition(q => q.Name == difficultySelect).First();
            Category category = _categoryRepository.FindByCondition(q => q.Name == categorySelect).First();
            QuestionType type = _questionTypeRepository.FindByCondition(q => q.Name == typeSelect).First();


            //Adds Entry to Player's quizes
            PlayerQuiz quiz = new PlayerQuiz();
            quiz.Player = player;
            quiz.Difficulty = difficulty;
            quiz.QuestionType = type;
            quiz.Category = category;
            quiz.Attempted = t.Questions.Results.Select(q => q.Selected).Count();
            quiz.Correct = t.Questions.Results.Where(q => q.Selected == "true").Count();
            _quizRepository.Create(quiz);



            //Adds Entry into Players Cash account
            PlayerCash playerCash = new PlayerCash();
            playerCash.ProfileId = player.Id;
            int multiplier = 0;
            switch (difficultySelect)
            {
                case "easy":
                    multiplier += quiz.Correct * (typeSelect == "Multiple Choice" ? 2 : 1);
                    break;
                case "medium":
                    multiplier += quiz.Correct * (typeSelect == "Multiple Choice" ? 2 : 1) * 2;
                    break;
                case "hard":
                    multiplier += quiz.Correct * (typeSelect == "Multiple Choice" ? 2 : 1) * 3;
                    break;
            }

            playerCash.Balance = _cashRepository
               .FindByCondition(p => p.ProfileId == player.Id)
               .OrderByDescending(c => c.CreatedDate)
               .Select(p => p.Balance)
               .FirstOrDefault();
            playerCash.Balance += (multiplier * 10);
            _cashRepository.Create(playerCash);


            //Adds Entry to Players Transactions
            PlayerTransaction playerTransaction = new PlayerTransaction();
            playerTransaction.ProfileId = player.Id;
            playerTransaction.TransactionType = "Trivia";
            playerTransaction.TransactionDescription = $"{quiz.Correct} ,{difficultySelect}, {typeSelect} questions";
            playerTransaction.Price = multiplier * 10;
            _transactionRepository.Create(playerTransaction);


            //Adds Entry to Players Prestige Score
            PlayerPrestigeScore playerPrestige = new PlayerPrestigeScore();
            playerPrestige.ProfileId = player.Id;
            playerPrestige.Source = "Knowledge Reward";
            playerPrestige.Score = _prestigeScoreRepository
            .FindByCondition(p => p.ProfileId == player.Id)
            .OrderByDescending(c => c.CreatedDate)
            .Select(p => p.Score)
            .FirstOrDefault();
            playerPrestige.Score += multiplier * 10;
            playerPrestige.PointsEarned = multiplier * 10;
            _prestigeScoreRepository.Create(playerPrestige);




            return RedirectToAction("Performance");
        }
       
        

    }
}
