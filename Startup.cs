using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThatsLife.Models;
using ThatsLife.Models.DAL;
using ThatsLife.Models.DAL.SeedData;
using ThatsLife.Models.Entity;

namespace ThatsLife
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApiHub.InitializeClient();
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("ThatsLifeConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthentication()
        .AddGoogle(options =>
        {
            IConfigurationSection googleAuthNSection =
                Configuration.GetSection("Authentication:Google");

            options.ClientId = googleAuthNSection["ClientId"];
            options.ClientSecret = googleAuthNSection["ClientSecret"];
        }).AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
        });


            
            services.AddScoped<IRepository<PlayerProfile>, ProfileRepository>();
            services.AddScoped<IRepository<PlayerStock>, StockRepository>();
            services.AddScoped<IRepository<PlayerTransaction>,TransactionRepository>();
            services.AddScoped<IRepository<PlayerCash>, CashRepository>();
            services.AddScoped<IRepository<PlayerPrestigeScore>, PrestigeScoreRepository>();
            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<IRepository<Difficulty>, DifficultyRepository>();
            services.AddScoped<IRepository<QuestionType>, QuestionTypeRepository>();
            services.AddScoped<IRepository<PlayerQuiz>, QuizRepository>();


            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews();
            services.AddRazorPages();

            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            CategorySeedData.EnsurePopulated(app);
            DifficultySeedData.EnsurePopulated(app);
            QuestionTypeSeedData.EnsurePopulated(app);
        }
    }
}
