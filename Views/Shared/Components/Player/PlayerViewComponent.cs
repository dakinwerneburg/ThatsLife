using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models;
using ThatsLife.Models.DAL;

namespace ThatsLife.Views.Shared.Components.Player
{
    public class PlayerViewComponent : ViewComponent
    {

        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public PlayerViewComponent(IRepository<PlayerProfile> profileRepository,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _profileRepository = profileRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            PlayerProfile player = _profileRepository.FindByCondition(p => p.UserId == user.Id).First();
            return View("Default", player );
        }
    }
}
