using GenericServices.Core.Abstracts;

namespace GenericServices.Core.Interfaces;
public interface IRootService<T, TId> where T : AggregateRoot<T,TId>
{
    Task<T> InsertAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<TId> DeleteAsync(TId entityId);
    IAsyncEnumerable<T> GetAllAsync(string includeEntities="");
    IAsyncEnumerable<T> GetAllAsync(Func<T, bool> predicate,string includeEntities="");
    Task<T> GetByIdAsync(TId entityId,string includeEntities="");

}
