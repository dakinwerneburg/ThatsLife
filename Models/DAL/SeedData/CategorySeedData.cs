using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class CategorySeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            ApplicationDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new Category
                {
                    TriviaDbCategoryId = 9,
                    Name = "General Knowledge"

                }, new Category
                {
                    TriviaDbCategoryId = 10,
                    Name = "Entertainment: Books"
                }, new Category
                {
                    TriviaDbCategoryId = 11,
                    Name = "Entertainment: Film"
                }, new Category
                {
                    TriviaDbCategoryId = 12,
                    Name = "Entertainment: Music"
                }, new Category
                {
                    TriviaDbCategoryId = 13,
                    Name = "Entertainment: Musicals & Theatres"
                }, new Category
                {
                    TriviaDbCategoryId = 14,
                    Name = "Entertainment: Television"
                }, new Category
                {
                    TriviaDbCategoryId = 15,
                    Name = "Entertainment: Video Games"
                }, new Category
                {
                    TriviaDbCategoryId = 16,
                    Name = "Entertainment: Board Games"
                }, new Category
                {
                    TriviaDbCategoryId = 17,
                    Name = "Science & Nature"
                }, new Category
                {
                    TriviaDbCategoryId = 18,
                    Name = "Science: Computers"
                }, new Category
                {
                    TriviaDbCategoryId = 19,
                    Name = "Science: Mathematics"
                }, new Category
                {
                    TriviaDbCategoryId = 20,
                    Name = "Mythology"
                }, new Category
                {
                    TriviaDbCategoryId = 21,
                    Name = "Sports"
                }, new Category
                {
                    TriviaDbCategoryId = 22,
                    Name = "Geography"
                }, new Category
                {
                    TriviaDbCategoryId = 23,
                    Name = "History"
                }, new Category
                {
                    TriviaDbCategoryId = 24,
                    Name = "Politics"
                }, new Category
                {
                    TriviaDbCategoryId = 25,
                    Name = "Art"
                }, new Category
                {
                    TriviaDbCategoryId = 26,
                    Name = "Celebrities"
                }, new Category
                {
                    TriviaDbCategoryId = 27,
                    Name = "Animals"
                }, new Category
                {
                    TriviaDbCategoryId = 28,
                    Name = "Vehicles"
                }, new Category
                {
                    TriviaDbCategoryId = 29,
                    Name = "Entertainment: Comics"
                }, new Category
                {
                    TriviaDbCategoryId = 30,
                    Name = "Science: Gadgets"
                }, new Category
                {
                    TriviaDbCategoryId = 31,
                    Name = "Entertainment: Japanese Anime & Manga"
                }, new Category
                {
                    TriviaDbCategoryId = 32,
                    Name = "Entertainment: Cartoon & Animations"
                });
                context.SaveChanges();
            }
        }
    }
}