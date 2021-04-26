using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public class StockRepository : IRepository<PlayerStock>
    {
        private readonly ApplicationDbContext _Context;
        public StockRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public void Create(PlayerStock entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerStock entity)
        {
            _Context.Remove(entity);
            _Context.SaveChanges();
        }

        public IQueryable<PlayerStock> FindAll()
        {
            return _Context.PlayerStocks;
        }

        public IQueryable<PlayerStock> FindByCondition(Expression<Func<PlayerStock, bool>> expression)
        {
            return _Context.PlayerStocks.Where(expression);
        }


        public void Update(PlayerStock entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
