using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class DifficultyRepository : IRepository<Difficulty>
    {
        private readonly ApplicationDbContext _Context;
        public DifficultyRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public void Create(Difficulty entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(Difficulty entity)
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

        public IQueryable<Difficulty> FindAll()
        {
            return _Context.Difficulties;
        }

        public IQueryable<Difficulty> FindByCondition(System.Linq.Expressions.Expression<Func<Difficulty, bool>> expression)
        {
            return _Context.Difficulties.Where(expression);
        }

        public void Update(Difficulty entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
