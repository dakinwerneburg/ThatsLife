using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        public DbSet<PlayerStock> PlayerStocks { get; set; }
        public DbSet<PlayerTransaction> PlayerTransactions {get;set;}
        public DbSet<PlayerCash> PlayerCash { get; set; }
        public DbSet<PlayerPrestigeScore> PlayerPrestigeScores { get; set; }


        /// <summary>
        /// Adds functionality to automatically add dates when a record is created or updated.
        /// https://www.entityframeworktutorial.net/faq/set-created-and-modified-date-in-efcore.aspx
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
