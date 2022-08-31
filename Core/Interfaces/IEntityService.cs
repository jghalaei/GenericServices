
using GenericServices.Core.Abstracts;

namespace GenericServices.Core.Interfaces;
public interface IEntityService<ParentId, T, Id> where T : AggregateEntity<Id>
{
    Task<T> InsertAsync(ParentId parentId, T entity);
    Task<T> UpdateAsync(ParentId parentId, T entity);
    Task<Id> DeleteAsync(ParentId parentId, Id entityId);
    IAsyncEnumerable<T> GetAllAsync(ParentId parentId);
        IAsyncEnumerable<T> GetAllAsync(ParentId parentId,Func<T,bool> predicate);
    Task<T> GetByIdAsync(ParentId parentId, Id entityId);
}
