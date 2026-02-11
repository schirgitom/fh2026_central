using System.Linq.Expressions;
using Services.Results;

namespace Services.Interfaces;

public interface IService<TEntity>
{
    Task<ServiceResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(string id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default);
}
