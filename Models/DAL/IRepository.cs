using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public interface IRepository<T>
    {
        IQueryable<T> FindAll();
        Task<IQueryable<T>> FindAllAsync();
        
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<IQueryable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);

        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
