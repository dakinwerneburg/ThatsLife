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

namespace ThatsLife.Views.Shared.Components.Player
{
    public class PlayerViewComponent : ViewComponent
    {

        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerCash> _cashRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigescoreRepository;

        public PlayerViewComponent(IRepository<PlayerProfile> profileRepository,
            UserManager<IdentityUser> userManager,
            IRepository<PlayerCash> cashRepository,
            IRepository<PlayerPrestigeScore> prestigescoreRepository)

        {
            _profileRepository = profileRepository;
            _cashRepository = cashRepository;
            _prestigescoreRepository = prestigescoreRepository;
            _userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            PlayerProfile player = _profileRepository.FindByCondition(p => p.UserId == user.Id).First();
            PlayerViewModel playerViewModel = new PlayerViewModel();
            
            playerViewModel.ProfileName = player.ProfileName;
            playerViewModel.Score = _prestigescoreRepository
                .FindByCondition(p => p.ProfileId == player.Id)
                .OrderByDescending(c => c.CreatedDate)
                .Select(p => p.Score)
                .FirstOrDefault();
            playerViewModel.Cash = _cashRepository
               .FindByCondition(p => p.ProfileId == player.Id)
               .OrderByDescending(c => c.CreatedDate)
               .Select(p => p.Balance)
               .FirstOrDefault();
            return View("Default", playerViewModel );
        }
    }
}
