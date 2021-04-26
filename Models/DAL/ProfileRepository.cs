using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public class ProfileRepository : IRepository<PlayerProfile>
    {
        private readonly ApplicationDbContext _Context;

        public ProfileRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public void Create(PlayerProfile entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerProfile entity)
        {
            _Context.Remove(entity);
            _Context.SaveChanges();
        }

        public IQueryable<PlayerProfile> FindAll()
        {
            return _Context.PlayerProfiles;
        }

        public IQueryable<PlayerProfile> FindByCondition(Expression<Func<PlayerProfile, bool>> expression)
        {
            return _Context.PlayerProfiles.Where(expression);
        }

        
        public void Update(PlayerProfile entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }


        public PlayerProfile GetProfileById(string id)
        {
            return _Context.PlayerProfiles.FirstOrDefault(p => p.UserId == id);
        }
    }
}
