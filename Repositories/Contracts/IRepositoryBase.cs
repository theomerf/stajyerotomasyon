using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> FindAll(bool trackChanges);

        IQueryable<T?> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

        void Create(T entity);

        void Remove(T entity);

        void Update(T entity);

        Task<int> Count(bool trackChanges);
        void AttachRange(IEnumerable<T> entities);
        void Attach(T entity);
        EntityEntry<T> Entry(T entity);
        void AddRange(IEnumerable<T> entities);
    }
}
