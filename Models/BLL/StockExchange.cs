using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.DAL;

namespace ThatsLife.Models
{
    public class StockExchange
    {
        public StockExchange(
                       SignInManager<IdentityUser> signInManager,
                       UserManager<IdentityUser> userManager,
                       IRepository<PlayerProfile> profileRepository,
                       IRepository<PlayerStock> stockRepository,
                       IRepository<PlayerTransaction> transactionRepository,
                       IConfiguration configuration
                       )
        {

        }
    }
}
