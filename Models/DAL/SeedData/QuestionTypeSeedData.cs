using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL.SeedData
{
    public class QuestionTypeSeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.QuestionTypes.Any())
            {
                context.QuestionTypes.AddRange(new QuestionType
                {
                    Name = "Multiple Choice"
                }, new QuestionType
                {
                    Name = "True/False"
                });
                context.SaveChanges();
            }
        }
    }
}
