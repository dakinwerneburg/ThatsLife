using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class QuestionTypeRepository : IRepository<QuestionType>
    {

        private readonly ApplicationDbContext _Context;
        public QuestionTypeRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public void Create(QuestionType entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(QuestionType entity)
        {
            _Context.Remove(entity);
            while (true)
                try
                {
                    _Context.SaveChanges();
                    break;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        entry.Reload();
                    }
                }
        }

        public IQueryable<QuestionType> FindAll()
        {
            return _Context.QuestionTypes;
        }

        public IQueryable<QuestionType> FindByCondition(Expression<Func<QuestionType, bool>> expression)
        {
            return _Context.QuestionTypes.Where(expression);
        }

        public void Update(QuestionType entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
