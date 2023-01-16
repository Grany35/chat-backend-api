using Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, new()
        where TContext : DbContext, new()
    {
        public async Task AddAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var addedEntity = context.Entry(entity);
                addedEntity.State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var addedEntity = context.Entry(entity);
                addedEntity.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            using (var context = new TContext())
            {
                IQueryable<TEntity> queryable = context.Set<TEntity>();
                if (include != null)
                {
                    queryable = include(queryable);
                }
                return filter == null
                    ? await queryable.ToListAsync()
                    : await queryable.Where(filter).ToListAsync();
            }
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            using (var context = new TContext())
            {
                IQueryable<TEntity> queryable = context.Set<TEntity>();
                if (include != null)
                {
                    queryable = include(queryable);
                }
                return await queryable.FirstOrDefaultAsync(filter);
            }
        }

        public async Task<IPaginate<TEntity>> GetListByPaginateAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, int index = 0, int size = 10, CancellationToken cancellationToken = default)
        {
            using(var context=new TContext())
            {
                IQueryable<TEntity> queryable = context.Set<TEntity>();
                if (include != null) queryable = include(queryable);
                if (predicate != null) queryable = queryable.Where(predicate);
                if (orderBy != null)
                    return await orderBy(queryable).ToPaginateAsync(index, size, 0, cancellationToken);
                return await queryable.ToPaginateAsync(index, size, 0, cancellationToken);
            }
            
        }

        public async Task UpdateAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                var addedEntity = context.Entry(entity);
                addedEntity.State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
        }
    }
}
