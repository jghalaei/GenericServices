using System.Text.Json;
using AutoMapper;
using GenericServices.Core.Abstracts;
using GenericServices.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GenericServices.Services.Controllers;


public abstract class AggregateRootController<Entity, ID, Model, InsertModel, UpdateModel> : ControllerBase where Entity : AggregateRoot<Entity,ID>
{
    private readonly IRootService<Entity, ID> _rootService;
    private readonly IMapper _mapper;
    private readonly ILogger<Entity> _logger;

    public AggregateRootController(IRootService<Entity, ID> service,ILogger<Entity> logger, IMapper mapper)
    {
        _rootService = service;
        _mapper = mapper;
        _logger = logger;
        
    }

    [HttpPost]
    public virtual async Task<IActionResult> InsertAsync( [FromBody] InsertModel model)
    {            
        _logger.LogInformation(MyLogEvents.InsertItem, "Inserting {model}....",JsonSerializer.Serialize(model));
        if (!ModelState.IsValid)
            {
                _logger.LogWarning(MyLogEvents.InsertItem, "Model Not Valid: {model}",JsonSerializer.Serialize(model));
                return BadRequest();
            }
        Entity entity = _mapper.Map<Entity>(model);
        var addedEntity = await _rootService.InsertAsync(entity);
        var addedModel = _mapper.Map<Model>(addedEntity);
            _logger.LogInformation(MyLogEvents.InsertItem, "...The object {entity} Inserted",JsonSerializer.Serialize(addedEntity));
        return CreatedAtAction(nameof(GetEntity), new {Id = addedEntity }, addedModel);
    }

    [HttpPut("{Id}")]
    public virtual async Task<ActionResult<Model>> UpdateAsync(ID Id, [FromBody] UpdateModel model)
    {
        _logger.LogInformation(MyLogEvents.InsertItem, "Updating {model}...",JsonSerializer.Serialize(model));

        if (!ModelState.IsValid)
           {
             _logger.LogWarning(MyLogEvents.UpdateItem, "Model Not Valid: {model}",JsonSerializer.Serialize(model));
             return BadRequest();
           }
        var entity = _mapper.Map<Entity>(model);
        var updatedEntity = await _rootService.UpdateAsync(entity);
        _logger.LogInformation(MyLogEvents.UpdateItem, "...The object updated: {entity} ",JsonSerializer.Serialize(updatedEntity));

        return Ok(updatedEntity);
    }

    [HttpDelete("{Id}")]
    public virtual async Task<ActionResult> DeleteAsync(ID Id)
    {
        _logger.LogInformation(MyLogEvents.DeleteItem, "Updating {model}...",Id);

        await _rootService.DeleteAsync(Id);
        _logger.LogInformation(MyLogEvents.DeleteItem, "...The object deleted: {id}",Id);

        return Ok();
    }

    [HttpGet("{Id}")]
    public  virtual async Task<ActionResult<Model>> GetEntity(ID Id)
    {
        _logger.LogInformation(MyLogEvents.GetItem, "Getting Item {model}...",Id);
        var entity = await _rootService.GetByIdAsync(Id);
        var model = _mapper.Map<Model>(entity);
        _logger.LogInformation(MyLogEvents.GetItem, "Item Retrived: {entity}...",JsonSerializer.Serialize(entity));
        return Ok(entity);
    }
    [HttpGet]
    public virtual async IAsyncEnumerable<Model> GetAll()
    {
        int count=0;
         _logger.LogInformation(MyLogEvents.ListItems, "Getting Items...");
        await foreach (var entity in _rootService.GetAllAsync())
        {
            count++;
            yield return _mapper.Map<Model>(entity);
        }
        _logger.LogInformation(MyLogEvents.ListItems, "Getting {count} Items...",count);
    }

}
