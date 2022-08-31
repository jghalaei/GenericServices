using GenericServices.Core.Abstracts;
using GenericServices.Core.Services;

namespace GenericServices.Core.Services;
public  class ServiceBase<T, ID> where T : AggregateEntity<ID>
{
    protected virtual Task BeforeInsert(T addedEntity)
    {
        return Task.CompletedTask;
    }
    protected virtual Task BeforeUpdate(T updatedEntity)
    {
        return Task.CompletedTask;
    }
    protected virtual Task BeforeDelete(ID entityId)
    {
        return Task.CompletedTask;
    }

    protected virtual Task AfterInsert(T addedEntity)
    {
        return Task.CompletedTask;
    }
    protected virtual Task AfterUpdate(T updatedEntity)
    {
        return Task.CompletedTask;
    }
    protected virtual Task AfterDelete(ID entityId)
    {
        return Task.CompletedTask;
    }


    protected virtual Task<ServiceValidationResult> ValidateEntity(T entity)
    {
        var result = new ServiceValidationResult();
        return Task.FromResult(result);
    }
    protected virtual Func<T, bool> EqualityPredicate(T entity) 
    {
        return (e=> e.Id.Equals(entity.Id));
    }
    protected virtual Task<ServiceValidationResult> ValidateForInsert(T entity)
    {
        var result = new ServiceValidationResult();
        return Task.FromResult(result);
    }
    protected virtual Task<ServiceValidationResult> ValidateForUpdate(T entity)
    {
        var result = new ServiceValidationResult();
        return Task.FromResult(result);
    }
    protected virtual Task<ServiceValidationResult> ValidateForDelete(ID entity)
    {
        var result = new ServiceValidationResult();
        return Task.FromResult(result);
    }
}