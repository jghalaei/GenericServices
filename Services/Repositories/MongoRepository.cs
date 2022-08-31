
using System.Linq.Expressions;
using GenericServices.Core.Abstracts;
using GenericServices.Core.Exceptions;
using GenericServices.Core.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Services.Repositories;

namespace GenericServices.Services.Repositories;

public class MongoRepository<T, ID> : IRepository<T, ID> where T : AggregateRoot<T,ID>
{

    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
    private ILogger<T> _logger;

    public MongoRepository(MongoDbConfig mongoDbConfig, ILogger<T> logger)
    {
        _logger = logger;
        var mongoClient = new MongoClient(mongoDbConfig.ConnectionString);
        var database = mongoClient.GetDatabase(mongoDbConfig.DbName);
        dbCollection = database.GetCollection<T>(mongoDbConfig.CollectionName);

    }
    public virtual async Task<T> InsertAsync(T aggregate)
    {
        try
        {
            aggregate.CreatedAt=DateTime.Now;
            await dbCollection.InsertOneAsync(aggregate);
            return aggregate;
        }
        catch (Exception ex)
        {
            throw new DbInsertException(ex.Message, aggregate);
        }
    }

    public virtual async Task<T> UpdateAsync(T aggregate)
    {
        try
        {
            aggregate.UpdatedAt=DateTime.Now;
            return await dbCollection.FindOneAndReplaceAsync(getFilterById(aggregate.Id), aggregate);
        }
        catch (Exception ex)
        {
            throw new DbUpdateException(ex.Message, aggregate);
        }
    }

    private FilterDefinition<T> getFilterById(ID id)
    {
        return filterBuilder.Eq(entity => entity.Id, id);
    }

    public virtual async Task<ID> DeleteAsync(ID AggregateId)
    {
        try
        {
            await dbCollection.FindOneAndDeleteAsync(getFilterById(AggregateId));
            return AggregateId;
        }
        catch (Exception ex)
        {

            throw new DbDelteException(ex.Message, AggregateId);
        }

    }
    public virtual async IAsyncEnumerable<T> GetAllAsync(Func<T, bool> predicate, string includeEntities = "")
    {
        if (includeEntities != "")
            throw new InvalidDataException("Include Entities does not supported in Mongo Repository");
        var cursor = dbCollection.AsQueryable().Where(predicate).ToAsyncEnumerable().GetAsyncEnumerator();
        while (await cursor.MoveNextAsync())
        {
            yield return cursor.Current;
        }
    }
    public virtual async IAsyncEnumerable<T> GetAllAsync(string includeEntities = "")
    {
        if (includeEntities != "")
            throw new InvalidDataException("Include Entities does not supported in Mongo Repository");
        await foreach (var item in GetAllAsync(t => 1 == 1))
        {
            yield return item;
        }
    }

    public virtual async Task<T> GetByIDAsync(ID id, string includeEntities = "")
    {
        if (includeEntities != "")
            throw new InvalidDataException("Include Entities does not supported in Mongo Repository");
        try
        {
            var result = (await dbCollection.FindAsync(getFilterById(id))).FirstOrDefault();
            return result;
        }
        catch (Exception ex)
        {
            throw new DbSelectException(ex.Message);
        }
    }


}
