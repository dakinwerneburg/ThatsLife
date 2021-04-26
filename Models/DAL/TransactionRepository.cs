using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public class TransactionRepository : IRepository<PlayerTransaction>
    {

        private readonly ApplicationDbContext _Context;
        public TransactionRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public void Create(PlayerTransaction entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerTransaction entity)
        {
            _Context.Remove(entity);
            _Context.SaveChanges();
        }

        public IQueryable<PlayerTransaction> FindAll()
        {
            return _Context.PlayerTransactions;
        }

        public IQueryable<PlayerTransaction> FindByCondition(Expression<Func<PlayerTransaction, bool>> expression)
        {
            return _Context.PlayerTransactions.Where(expression);
        }

        public void Update(PlayerTransaction entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
