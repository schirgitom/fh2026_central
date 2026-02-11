using DAL.Entities;
using System.Linq.Expressions;

namespace DAL.Repository
{
    public interface IRepository<TEntity> where TEntity : Entity
    {

        Task<List<TEntity>> FilterAsync(
           Expression<Func<TEntity, bool>> filterExpression,
           CancellationToken cancellationToken = default);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterExpression,
            CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(string id,
            CancellationToken cancellationToken = default);

        Task<TEntity> InsertAsync(TEntity document,
            CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(string id, TEntity document,
            CancellationToken cancellationToken = default);


        Task DeleteAsync(Expression<Func<TEntity, bool>> filterExpression,
            CancellationToken cancellationToken = default);

    
    }
}
