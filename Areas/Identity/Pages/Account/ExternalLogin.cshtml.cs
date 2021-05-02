using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;

namespace ThatsLife.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly IRepository<PlayerCash> _cashRepository;
        private readonly IRepository<PlayerTransaction> _transactionRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigescoreRepository;

        public ExternalLoginModel(
             UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
           IRepository<PlayerProfile> profileRepository,
           IRepository<PlayerCash> cashRepository,
           IRepository<PlayerTransaction> tranactionRepository,
           IRepository<PlayerPrestigeScore> prestigescoreRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepository = profileRepository;
            _cashRepository = cashRepository;
            _transactionRepository = tranactionRepository;
            _prestigescoreRepository = prestigescoreRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);


                        PlayerProfile player = new PlayerProfile();
                        player.ProfileName = User.Identity.Name;
                        player.UserId = user.Id;
                        _profileRepository.Create(player);
                        player = _profileRepository.FindByCondition(p => p.UserId == user.Id).FirstOrDefault();


                        PlayerCash playerCash = new PlayerCash();
                        playerCash.ProfileId = player.Id;
                        playerCash.Balance = 1000000;
                        _cashRepository.Create(playerCash);

                        PlayerTransaction playerTransaction = new PlayerTransaction();
                        playerTransaction.Price = 1000000;
                        playerTransaction.ProfileId = player.Id;
                        playerTransaction.TransactionType = "Credit";
                        playerTransaction.TransactionDescription = "Player Created";
                        _transactionRepository.Create(playerTransaction);

                        PlayerPrestigeScore playerPrestigeScore = new PlayerPrestigeScore();
                        playerPrestigeScore.ProfileId = player.Id;
                        playerPrestigeScore.Score = 0;
                        playerPrestigeScore.Source = "Player Created";
                        _prestigescoreRepository.Create(playerPrestigeScore);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
