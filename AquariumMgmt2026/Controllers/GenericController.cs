using System.Linq.Expressions;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Results;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class GenericController<TEntity> : ControllerBase, IService<TEntity>
    where TEntity : Entity
{
    protected readonly IService<TEntity> Service;
    protected readonly ILogger Logger;

    protected GenericController(IService<TEntity> service, ILogger logger)
    {
        Service = service;
        Logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching all entities for {EntityType}", typeof(TEntity).Name);
        var entities = await Service.GetAllAsync(cancellationToken);
        Logger.LogInformation("Fetched {Count} entities for {EntityType}", entities.Count, typeof(TEntity).Name);
        return Ok(entities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
        var entity = await Service.GetAsync(id, cancellationToken);
        if (entity == null)
        {
            Logger.LogWarning("{EntityType} with ID {EntityId} was not found", typeof(TEntity).Name, id);
        }

        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        entity.ID = Guid.NewGuid().ToString();
        Logger.LogInformation("Creating {EntityType} with ID {EntityId}", typeof(TEntity).Name, entity.ID);
        var result = await Service.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            Logger.LogWarning("Failed to create {EntityType} with ID {EntityId}. Errors={ErrorCount}",
                typeof(TEntity).Name, entity.ID, result.Errors.Count);
        }

        return ResultToAction(result, created: true);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Updating {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
        entity.ID = id;
        var result = await Service.UpdateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            Logger.LogWarning("Failed to update {EntityType} with ID {EntityId}. Errors={ErrorCount}",
                typeof(TEntity).Name, id, result.Errors.Count);
        }

        return ResultToAction(result, created: false);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Deleting {EntityType} with ID {EntityId}", typeof(TEntity).Name, id);
        var result = await Service.DeleteAsync(id, cancellationToken);
        if (!result.IsSuccess)
        {
            Logger.LogWarning("Failed to delete {EntityType} with ID {EntityId}. Errors={ErrorCount}",
                typeof(TEntity).Name, id, result.Errors.Count);
        }

        return ResultToAction(result, created: false);
    }

    [NonAction]
    public Task<ServiceResult<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => Service.CreateAsync(entity, cancellationToken);

    [NonAction]
    public Task<ServiceResult<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => Service.UpdateAsync(entity, cancellationToken);

    [NonAction]
    public Task<ServiceResult<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
        => Service.DeleteAsync(id, cancellationToken);

    [NonAction]
    public Task<TEntity?> GetAsync(string id, CancellationToken cancellationToken = default)
        => Service.GetAsync(id, cancellationToken);

    [NonAction]
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => Service.GetAllAsync(cancellationToken);

    [NonAction]
    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filterExpression,
        CancellationToken cancellationToken = default)
        => Service.WhereAsync(filterExpression, cancellationToken);

    private IActionResult ResultToAction<T>(ServiceResult<T> result, bool created)
    {
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Errors, result.Warnings });
        }

        if (created)
        {
            return Ok(result.Data);
        }

        return Ok(result.Data);
    }
}
