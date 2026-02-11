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

    protected GenericController(IService<TEntity> service)
    {
        Service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var entity = await Service.GetAsync(id, cancellationToken);
        return entity == null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        var result = await Service.CreateAsync(entity, cancellationToken);
        return ResultToAction(result, created: true);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TEntity entity, CancellationToken cancellationToken)
    {
        entity.ID = id;
        var result = await Service.UpdateAsync(entity, cancellationToken);
        return ResultToAction(result, created: false);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await Service.DeleteAsync(id, cancellationToken);
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
