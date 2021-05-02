using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class PrestigeScoreRepository : IRepository<PlayerPrestigeScore>
    {
        private readonly ApplicationDbContext _Context;
        public PrestigeScoreRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public void Create(PlayerPrestigeScore entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerPrestigeScore entity)
        {
            _Context.Remove(entity);
            _Context.SaveChanges();
        }

        public IQueryable<PlayerPrestigeScore> FindAll()
        {
            return _Context.PlayerPrestigeScores;
        }

        public IQueryable<PlayerPrestigeScore> FindByCondition(Expression<Func<PlayerPrestigeScore, bool>> expression)
        {
            return _Context.PlayerPrestigeScores.Where(expression);
        }

        public void Update(PlayerPrestigeScore entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
