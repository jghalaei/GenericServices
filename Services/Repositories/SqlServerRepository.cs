using GenericServices.Core.Abstracts;
using GenericServices.Core.Exceptions;
using GenericServices.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Services.Repositories;

public class SqlServerRepository<T, ID> : IRepository<T, ID> where T : AggregateRoot<T,ID>
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public SqlServerRepository(DbContext dbContext)
    {

        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();

    }

    public async Task<T> InsertAsync(T aggregate)
    {
        try
        {
            aggregate.CreatedAt=DateTime.Now;
            var addedItem = await _dbSet.AddAsync(aggregate);
            await _dbContext.SaveChangesAsync();
            return addedItem.Entity;
        }
        catch (Exception ex)
        {
            throw new DbInsertException(ex.Message, aggregate);
        }
    }

    public async Task<T> UpdateAsync(T aggregate)
    {
        try
        {
            var oldItem = await _dbSet.FindAsync(aggregate.Id);
            if (oldItem == null)
                throw new EntityNotFoundException("The Item is not found");
           oldItem.UpdatedAt=DateTime.Now;
            oldItem.UpdateValues(aggregate);
            _dbSet.Update(oldItem);
            await _dbContext.SaveChangesAsync();
            return aggregate;
        }
        catch (Exception ex)
        {
            throw new Core.Exceptions.DbUpdateException(ex.Message, aggregate);

        }
    }
    public async Task<ID> DeleteAsync(ID AggregateId)
    {
        try
        {
            var item = await _dbSet.FindAsync(AggregateId);
            if (item == null)
                throw new EntityNotFoundException("The Item is not found");
            _dbSet.Remove(item);
            await _dbContext.SaveChangesAsync();
            return AggregateId;
        }
        catch (Exception ex)
        {

            throw new DbDelteException(ex.Message, AggregateId);
        }
    }

    public IAsyncEnumerable<T> GetAllAsync(string includeEntities = "")
    {
        return GetAllAsync((i => true), includeEntities);
    }
    public IAsyncEnumerable<T> GetAllAsync(Func<T, bool> predicate, string includeEntities = "")
    {
        if (includeEntities == "")
            return _dbSet.Where(predicate).ToAsyncEnumerable();

        return _dbSet.Include(includeEntities).Where(predicate).ToAsyncEnumerable();
    }

    public async Task<T> GetByIDAsync(ID Id, string includeEntities = "")
    {
        try
        {
            T? item;
            if (includeEntities == "")
                item = await _dbSet.FirstOrDefaultAsync(i => Id.Equals(i.Id));
            else
                item = await _dbSet.Include(includeEntities).FirstOrDefaultAsync(i => Id.Equals(i.Id));
            return item;
        }
        catch (Exception ex)
        {
            throw new DbSelectException(ex.Message);
        }
    }


}