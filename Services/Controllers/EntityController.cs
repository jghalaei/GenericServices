using System.Text.Json;
using AutoMapper;
using GenericServices.Core.Abstracts;
using GenericServices.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GenericServices.Services.Controllers;


public abstract class EntityController<Entity, ParentId, ID, Model, InsertModel, UpdateModel> : ControllerBase where Entity : AggregateEntity<ID>
{
    private readonly IEntityService<ParentId, Entity, ID> _entityService;
    private readonly IMapper _mapper;
    private readonly ILogger<Entity> _logger;

    public EntityController(IEntityService<ParentId, Entity, ID> service, ILogger<Entity> logger, IMapper mapper)
    {
        _entityService = service;
        _mapper = mapper;
        _logger = logger;
        


    }

    [HttpPost]
    public virtual async Task<IActionResult> InsertAsync(ParentId parentId, [FromBody] InsertModel model)
    {
        _logger.LogInformation(MyLogEvents.InsertItem, "Inserting {model}....", JsonSerializer.Serialize(model));
        if (!ModelState.IsValid)
        {
            _logger.LogWarning(MyLogEvents.InsertItem, "Model Not Valid: {model}", JsonSerializer.Serialize(model));
            return BadRequest();
        }
        Entity entity = _mapper.Map<Entity>(model);
        var addedEntity = await _entityService.InsertAsync(parentId, entity);
        _logger.LogInformation(MyLogEvents.InsertItem, "...The object {entity} Inserted", JsonSerializer.Serialize(addedEntity));
        return CreatedAtAction(nameof(GetEntity), new { parentId = parentId, Id = addedEntity.Id }, addedEntity);
    }

    [HttpPut]
    public virtual async Task<ActionResult<Model>> UpdateAsync(ParentId parentId, [FromBody] UpdateModel model)
    {
               _logger.LogInformation(MyLogEvents.InsertItem, "Updating {model}...",JsonSerializer.Serialize(model));

        if (!ModelState.IsValid)
        {
            _logger.LogWarning(MyLogEvents.UpdateItem, "Model Not Valid: {model}",JsonSerializer.Serialize(model));
             return BadRequest();
        }
        var entity = _mapper.Map<Entity>(model);
        var updatedEntity = await _entityService.UpdateAsync(parentId, entity);
        _logger.LogInformation(MyLogEvents.UpdateItem, "...The object updated: {entity} ",JsonSerializer.Serialize(updatedEntity));

        return Ok(updatedEntity);
    }

    [HttpDelete("{Id}")]
    public virtual async Task<ActionResult> DeleteAsync(ParentId parentId, ID Id)
    {
               _logger.LogInformation(MyLogEvents.DeleteItem, "Updating {model}...",Id);

        await _entityService.DeleteAsync(parentId, Id);
                _logger.LogInformation(MyLogEvents.DeleteItem, "...The object deleted: {id}",Id);

        return Ok();
    }

    [HttpGet("{Id}")]
    public virtual async Task<ActionResult<Model>> GetEntity(ParentId parentId, ID Id)
    {
                _logger.LogInformation(MyLogEvents.GetItem, "Getting Item {model}...",Id);

        var entity = await _entityService.GetByIdAsync(parentId, Id);
        var model = _mapper.Map<Model>(entity);
                _logger.LogInformation(MyLogEvents.GetItem, "Item Retrived: {entity}...",JsonSerializer.Serialize(entity));

        return Ok(entity);
    }
    [HttpGet]
    public virtual async IAsyncEnumerable<Model> GetAll(ParentId parentId)
    {
                int count=0;
        _logger.LogInformation(MyLogEvents.ListItems, "Getting Items...");
        await foreach (var entity in _entityService.GetAllAsync(parentId))
        {
            count++;
            yield return _mapper.Map<Model>(entity);
        }
         _logger.LogInformation(MyLogEvents.ListItems, "Getting {count} Items...",count);

    }

}
