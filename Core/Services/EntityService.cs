using GenericServices.Core.Abstracts;
using GenericServices.Core.Exceptions;
using GenericServices.Core.Interfaces;
using GenericServices.Core.Services;

namespace GenericServices.Core.Services;
public abstract class EntityService<ROOT, ROOTID, T, ID> : ServiceBase<T, ID>, IEntityService<ROOTID, T, ID>
                        where T : AggregateEntity<ID>
                        where ROOT : AggregateRoot<ROOT,ROOTID>

{
    private readonly IRepository<ROOT, ROOTID> _repository;

    protected EntityService(IRepository<ROOT, ROOTID> repository)
    {
        _repository = repository;
    }


    public virtual  async Task<T> InsertAsync(ROOTID parentId, T entity)
    {
        await BeforeInsert(entity);
        
        await CheckForInsert(entity);

        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>().ToList();
        if (entities.Any(EqualityPredicate(entity)))
            throw new InvalidEntityException("an entity with the same data is already exist.", entity);
        entities.Add(entity);
        await _repository.UpdateAsync(aggRoot);
        return entity;
    }

    private  async Task CheckForInsert(T entity)
    {
        var validationEntityResult = await ValidateEntity(entity);
        if (!validationEntityResult.IsValid)
            validationEntityResult.ThrowError();

        ServiceValidationResult result = await ValidateForInsert(entity);
        if (!result.IsValid)
            result.ThrowError();
    }

    private  async Task<ROOT> FindAggRoot(ROOTID parentId)
    {
        ROOT? aggRoot = await _repository.GetByIDAsync(parentId);
        if (aggRoot == null)
            throw new EntityNotFoundException($"The parent {typeof(ROOT).Name} with the given Id was not found", parentId);
        return aggRoot;
    }

    public virtual async Task<T> UpdateAsync(ROOTID parentId, T entity)
    {
        await BeforeUpdate(entity);
        await CheckForUpdate(entity);
        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>();
        if (entities == null || entities.Count() == 0)
        {
            throw new EntityNotFoundException("Entity Not Found");
        }
        T updatingItem = entities.FirstOrDefault<T>(e => e.Id.ToString() == entity.Id.ToString()) ?? throw new EntityNotFoundException($"{typeof(T).Name} was not found", entity.Id);
        updatingItem = entity;
        var updatedAggRoot = await _repository.UpdateAsync(aggRoot);
        await AfterUpdate(entity);
        return updatingItem;
    }

    private  async Task CheckForUpdate(T entity)
    {
        var validationEntityResult = await ValidateEntity(entity);
        if (!validationEntityResult.IsValid)
            validationEntityResult.ThrowError();

        var validationResult = await ValidateForUpdate(entity);
        if (!validationResult.IsValid)
            validationResult.ThrowError();
    }

    public virtual async Task<ID> DeleteAsync(ROOTID parentId, ID entityId)
    {
        await BeforeDelete(entityId);
        var validationResult = await ValidateForDelete(entityId);
        if (!validationResult.IsValid)
            validationResult.ThrowError();
        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>();
        T item = entities.FirstOrDefault<T>(e => e.Id.ToString() == entityId.ToString()) ?? throw new EntityNotFoundException($"{typeof(T).Name} was not found", entityId);
        entities.Remove(item);
        await _repository.UpdateAsync(aggRoot);
        return entityId;
    }

    public virtual async IAsyncEnumerable<T> GetAllAsync(ROOTID parentId)
    {
        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>();
        foreach (var item in entities)
        {
            yield return item;
        }
    }

    public virtual  async IAsyncEnumerable<T> GetAllAsync(ROOTID parentId, Func<T, bool> predicate)
    {
        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>().Where(predicate);
        foreach (var item in entities)
        {
            yield return item;
        }
    }

    public virtual async Task<T> GetByIdAsync(ROOTID parentId, ID entityId)
    {
        ROOT aggRoot = await FindAggRoot(parentId);
        var entities = aggRoot.GetEntities<T>();
        T item = entities.FirstOrDefault<T>(e => e.Id.ToString() == entityId.ToString()) ?? throw new EntityNotFoundException($"{typeof(T).Name} was not found", entityId);
        return item;
    }

}