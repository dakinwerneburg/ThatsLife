using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.Entity;

namespace ThatsLife.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRepository<PlayerProfile> _profileRepository;
        private readonly IRepository<PlayerCash> _cashRepository;
        private readonly IRepository<PlayerTransaction> _transactionRepository;
        private readonly IRepository<PlayerPrestigeScore> _prestigescoreRepository;


        public RegisterModel(
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

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    //Creates player after they are authenticated and not in database
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
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
