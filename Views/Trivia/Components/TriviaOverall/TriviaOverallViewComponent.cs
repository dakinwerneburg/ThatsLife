using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;
using Microsoft.AspNetCore.Http;
using ThatsLife.Models;
using Newtonsoft.Json;
using ThatsLife.Models.ViewModels;

namespace ThatsLife.Views.Trivia.Components.TriviaOverall
{
    public class TriviaOverallViewComponent : ViewComponent
    {
        private readonly IRepository<PlayerQuiz> _quizRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _profileRepository;

        public TriviaOverallViewComponent(IRepository<PlayerQuiz> quizRepository,
            UserManager<IdentityUser> userManager,
            IRepository<PlayerProfile> profileRepository)
        {
            _quizRepository = quizRepository;
            _userManager = userManager;
            _profileRepository = profileRepository;
        }
        
        public IViewComponentResult Invoke()

        {
            var userId = _userManager.GetUserId(Request.HttpContext.User);
            PlayerProfile player = _profileRepository.FindByCondition(p => p.UserId == userId).First();

            int correct = _quizRepository.FindByCondition(p => p.Player == player).Select(c => c.Correct).Sum();
            int attempted = _quizRepository.FindByCondition(p => p.Player == player).Select(c => c.Attempted).Sum();
            int incorrect = attempted - correct;

            var chartData = @"
            {
                type: 'pie',
                responsive: true,
                data:
                {
                    labels: ['Correct','Incorrect'],
                    datasets: [{
                        label: 'Overall Trivia Performance',
                        data: [" + correct + "," + incorrect + "]," +
                        "backgroundColor: [ 'rgba(144,238,144, 0.3)', 'rgba(255,114,111,0.3)']," +
                        "hoverOffset:4"+
                    "}]"+
                "}" +
            "}";

            var chart = JsonConvert.DeserializeObject<TriviaOverallChart>(chartData);
            var chartModel = new OverallChartViewModel
            {
                Chart = chart,
                ChartJson = JsonConvert.SerializeObject(chart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            };

            return View(chartModel);
        }
    }
}
