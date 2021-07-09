using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.DAL
{
    public class CategoryRepository : IRepository<Category>
    {

        private readonly ApplicationDbContext _Context;
        public CategoryRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public void Create(Category entity)
        {
            _Context.Add(entity);
            _Context.SaveChanges();
        }

        public void Delete(Category entity)
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

        public IQueryable<Category> FindAll()
        {
            return _Context.Categories;
        }

        public IQueryable<Category> FindByCondition(Expression<Func<Category, bool>> expression)
        {
            return _Context.Categories.Where(expression);
        }

        public void Update(Category entity)
        {
            _Context.Update(entity);
            _Context.SaveChanges();
        }
    }
}
