using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Controllers
{
    public class HomeController : Controller
    {
           private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User)){
                return RedirectToAction("exchange", "stock");
            }
            else
            {
                return View();
            }
            
        }

        
        public IActionResult About()
        {
            return View();
        }
    }
}
