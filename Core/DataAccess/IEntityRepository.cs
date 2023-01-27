using System.Linq.Expressions;
using Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore.Query;

namespace Core.DataAccess
{
    public interface IEntityRepository<T>
    {
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<T> GetAsync(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<IPaginate<T>> GetListByPaginateAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int index = 0, int size = 10,
            CancellationToken cancellationToken = default);
        
        IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = null);

    }
}