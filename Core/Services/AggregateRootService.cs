using GenericServices.Core.Abstracts;
using GenericServices.Core.Entities;
using GenericServices.Core.Exceptions;
using GenericServices.Core.Interfaces;


namespace GenericServices.Core.Services;
public class RootService<T, ID> : ServiceBase<T, ID>, IRootService<T, ID> where T : AggregateRoot<T,ID>
{

    private readonly IRepository<T, ID> _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly bool _publishMessages;

    public RootService(IRepository<T, ID> repository)
    {
        _repository = repository;
        _publishMessages = false;
    }

    public RootService(IRepository<T, ID> repository, IMessagePublisher messagePublisher, bool publishMessages = true)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _publishMessages = publishMessages;
    }

    public virtual async Task<T> InsertAsync(T aggregate)
    {
        await BeforeInsert(aggregate);
        await CheckForInsert(aggregate);

        var addedAggregate = await _repository.InsertAsync(aggregate);
        await AfterInsert(addedAggregate);
        if (_publishMessages)
            await _messagePublisher.PublishAsync<T>(typeof(T).Name, aggregate,EventType.Insert);
        return addedAggregate;
    }

    private async Task CheckForInsert(T aggregate)
    {
        var validationEntityResult = await ValidateEntity(aggregate);
        if (!validationEntityResult.IsValid)
            validationEntityResult.ThrowError();

        var validationResult = await ValidateForInsert(aggregate);
        if (!validationResult.IsValid)
            validationResult.ThrowError();

        var res = GetAllAsync(EqualityPredicate(aggregate)).GetAsyncEnumerator();
        if (await res.MoveNextAsync())
            throw new InvalidEntityException("an entity with the same data is already exist.", aggregate);
    }

    public virtual async Task<T> UpdateAsync(T aggregate)
    {
        await BeforeUpdate(aggregate);
        await CheckForUpdate(aggregate);
        T updatedAggregate = await _repository.UpdateAsync(aggregate);
        await AfterUpdate(updatedAggregate);
        if (_publishMessages)
            await _messagePublisher.PublishAsync<T>(typeof(T).Name, aggregate,EventType.Update);

        return updatedAggregate;
    }

    private async Task CheckForUpdate(T aggregate)
    {
        var validationEntityResult = await ValidateEntity(aggregate);
        if (!validationEntityResult.IsValid) validationEntityResult.ThrowError();

        var validationResult = await ValidateForUpdate(aggregate);
        if (!validationResult.IsValid)
            validationResult.ThrowError();
        await CheckExistance(aggregate.Id);
    }

    private async Task<T> CheckExistance(ID aggregateId)
    {
        var entity = await _repository.GetByIDAsync(aggregateId);
        if (entity == null)
            throw new EntityNotFoundException("Item with the given Id is not found", aggregateId);
        return entity;
    }

    public virtual async Task<ID> DeleteAsync(ID aggregateId)
    {
        await BeforeDelete(aggregateId);
        var validationResult = await ValidateForDelete(aggregateId);
        if (!validationResult.IsValid)
            validationResult.ThrowError();
        var aggregate = await CheckExistance(aggregateId);
        var id = await _repository.DeleteAsync(aggregateId);
        if (_publishMessages)
            await _messagePublisher.PublishAsync<T>(typeof(T).Name, aggregate,EventType.Delete);
        return id;
    }

    public virtual async IAsyncEnumerable<T> GetAllAsync(string includeEntities = "")
    {
        await foreach (var T in _repository.GetAllAsync(includeEntities))
        {
            yield return T;
        }
    }

    public virtual Task<T> GetByIdAsync(ID entityId, string includeEntities = "")
    {
        return (_repository.GetByIDAsync(entityId, includeEntities));
    }

    public virtual async IAsyncEnumerable<T> GetAllAsync(Func<T, bool> predicate, string includeEntities = "")
    {
        await foreach (var T in _repository.GetAllAsync(predicate, includeEntities))
        {
            yield return T;
        }
    }




}