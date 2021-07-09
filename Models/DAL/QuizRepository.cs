using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class QuizRepository : IRepository<PlayerQuiz>
    {

        private readonly ApplicationDbContext _Context;
        public QuizRepository(ApplicationDbContext context)
        {
            _Context = context;
        }


        public void Create(PlayerQuiz entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(PlayerQuiz entity)
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

        public IQueryable<PlayerQuiz> FindAll()
        {
            return _Context.PlayerQuizzes;
        }

        public IQueryable<PlayerQuiz> FindByCondition(Expression<Func<PlayerQuiz, bool>> expression)
        {
            return _Context.PlayerQuizzes.Where(expression);
        }

        public void Update(PlayerQuiz entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
