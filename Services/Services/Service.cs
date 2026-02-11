using System.Linq.Expressions;
using DAL.Entities;
using DAL.Repository;
using Services.Interfaces;
using Services.Results;
namespace Services.Services;

public class Service<TEntity> : IService<TEntity> where TEntity : Entity
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly DAL.UnitOfWork.IUnitOfWork UnitOfWork;

    public Service(IRepository<TEntity> repository, DAL.UnitOfWork.IUnitOfWork unitOfWork)
    {
        Repository = repository;
        UnitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = new ServiceResult<TEntity>();
        var errors = OnBeforeSave(entity, isCreate: true);
        if (errors.Count > 0)
        {
            result.Errors.AddRange(errors);
            return result;
        }

        await Repository.InsertAsync(entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        OnAfterSave(entity, isCreate: true);

        result.Data = entity;
        return result;
    }

    public async Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = new ServiceResult<TEntity>();
        var errors = OnBeforeSave(entity, isCreate: false);
        if (errors.Count > 0)
        {
            result.Errors.AddRange(errors);
            return result;
        }

        await Repository.UpdateAsync(entity.ID, entity, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);
        OnAfterSave(entity, isCreate: false);

        result.Data = entity;
        return result;
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = new ServiceResult<bool>();

        await Repository.DeleteAsync(e => e.ID.Equals(id), cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        result.Data = true;
        return result;
    }

    public async Task<TEntity?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Repository.GetAsync(id, cancellationToken);
    }

    public async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default)
    {
        return await Repository.FilterAsync(filterExpression, cancellationToken);
    }

    protected virtual List<string> OnBeforeSave(TEntity entity, bool isCreate) => new();

    protected virtual void OnAfterSave(TEntity entity, bool isCreate)
    {
    }
}
