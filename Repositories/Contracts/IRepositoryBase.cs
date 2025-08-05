using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trackChanges);

        IQueryable<T?> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

        void Create(T entity);

        void Remove(T entity);

        void Update(T entity);

        Task<int> Count(bool trackChanges);
    }
}
