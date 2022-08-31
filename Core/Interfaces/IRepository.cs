using GenericServices.Core.Abstracts;

namespace GenericServices.Core.Interfaces;

public interface IRepository<T, ID> where T : AggregateRoot<T,ID>
{
    public Task<T> InsertAsync(T aggregate);
    public Task<T> UpdateAsync(T aggregate);
    public Task<ID> DeleteAsync(ID AggregateId);
    public Task<T> GetByIDAsync(ID Id,string includeEntities="");
    public IAsyncEnumerable<T> GetAllAsync(string includeEntities="");
    public IAsyncEnumerable<T> GetAllAsync(Func<T,bool> predicate,string includeEntities="");
}
