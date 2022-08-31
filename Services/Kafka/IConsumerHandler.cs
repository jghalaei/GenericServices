using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Core.Entities;

namespace GenericServices.Services.Kafka
{
    public interface IMessageHandler<T>
    {
        public Task HandleMessage(T entity,EventType eventType);
        
    }    
}