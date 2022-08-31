using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Core.Abstracts;
using GenericServices.Core.Interfaces;
using GenericServices.Core.Services;
using GenericServices.Services.Controllers;
using GenericServices.Services.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Repositories;

namespace Services.Extensions
{
    public static class Extensions
    {
        public static void AddRootControllerWithMongoDB<TRoot, TId, TModel, TInsertModel, TUpdateModel>(this IServiceCollection services, MongoDbConfig mongoDbConfig, ERepository RepositoryType = ERepository.MongoDB, DbContext context = null) where TRoot : AggregateRoot<TRoot, TId>
        {
            services.AddScoped<MongoRepository<TRoot, TId>>(sp => new MongoRepository<TRoot, TId>(mongoDbConfig, sp.GetRequiredService<ILogger<TRoot>>()));
            if (!services.Any(sd => sd.ImplementationType == typeof(IRepository<TRoot, TId>)))
            {
                services.AddScoped<MongoRepository<TRoot, TId>>(sp => new MongoRepository<TRoot, TId>(mongoDbConfig, sp.GetRequiredService<ILogger<TRoot>>()));
            }
            services.AddScoped<IRootService<TRoot, TId>>(sp =>
            {
                var repo = sp.GetRequiredService<IRepository<TRoot,TId>>();
                var publisher = sp.GetRequiredService<IMessagePublisher>();
                if (publisher==null)
                return new RootService<TRoot,TId>(repo);
                else
                return new RootService<TRoot,TId>(repo,publisher);
            
            }
        }
    }
}