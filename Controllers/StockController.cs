using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        
        public IActionResult Exchange()
        {
            return View();
        }
    }
}
