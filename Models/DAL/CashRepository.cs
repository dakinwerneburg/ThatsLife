using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public class CashRepository : IRepository<PlayerCash>
    {
        private readonly ApplicationDbContext _Context;

        public CashRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public void Create(PlayerCash entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerCash entity)
        {
            _Context.Remove(entity);
            _Context.SaveChanges();
        }

        public IQueryable<PlayerCash> FindAll()
        {
            return _Context.PlayerCash;
        }

        public IQueryable<PlayerCash> FindByCondition(Expression<Func<PlayerCash, bool>> expression)
        {
            return _Context.PlayerCash.Where(expression);
        }

        public void Update(PlayerCash entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
