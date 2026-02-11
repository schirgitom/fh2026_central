using DAL.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly AquariumDBContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AquariumDBContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }
    

        public async Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(filterExpression)
                .ToListAsync(cancellationToken);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(filterExpression)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(e => e.ID.Equals(id))
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<TEntity> InsertAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(string id, TEntity document, CancellationToken cancellationToken = default)
        {
            DbSet.Update(document);
            return await Task.FromResult(document);
        }

       
        public async Task DeleteAsync(
            Expression<Func<TEntity, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var entities = await DbSet
                .Where(filterExpression)
                .ToListAsync(cancellationToken);

            DbSet.RemoveRange(entities);
        }
    }
}
