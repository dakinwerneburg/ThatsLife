using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;
using ThatsLife.Models.ViewModels;

namespace ThatsLife.Controllers
{
    public class AchievementController : Controller
    {
        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigeScoreRepository;
        public AchievementController(IRepository<PlayerProfile> profileRepository, 
            IRepository<PlayerPrestigeScore> prestigeScoreRepository)
        {
            _profileRepository = profileRepository;
            _prestigeScoreRepository = prestigeScoreRepository;
        }
        
        public IActionResult Leaderboard()
        {

            var leaderBoardViewModel = new LeaderBoardViewModel();
            //List<int?> profiles = _prestigeScoreRepository.FindAll().Select(p => p.ProfileId).Distinct().ToList();
            var innerJoin = _prestigeScoreRepository.FindAll()
                .Join(_profileRepository.FindAll(),
                playerScore => playerScore.ProfileId,
                playerProfile => playerProfile.Id,
                (playerScore, playerProfile) => new
                {
                    profileName = playerProfile.ProfileName,
                    pointsEarned = playerScore.PointsEarned,
                    totalScore = playerScore.Score,
                    source = playerScore.Source,
                    createDate = playerScore.CreatedDate
                }).GroupBy(p => new { p.profileName, p.source }).Select(group => new
                {
                    group.Key.profileName,
                    score = group.Max(d => d.totalScore)

                }).OrderByDescending(s => s.score).Select(s => new
                {
                    s.profileName,
                    s.score
                }).GroupBy(p => p.profileName).Select(s => new
                {
                    profileName = s.Key,
                    score = s.Max(m => m.score)
                }).OrderByDescending(o => o.score).ToList();

            List<PlayerRanking> playerRankings = new List<PlayerRanking>();
            foreach(var item in innerJoin)
            {
                PlayerRanking ranking = new PlayerRanking();
                ranking.ProfileName = item.profileName;
                ranking.Score = item.score;
                playerRankings.Add(ranking);
            }
            
            return View(playerRankings);
        }
    }
}
