using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Core.Entities;

namespace GenericServices.Core.Interfaces
{
    public interface IMessagePublisher
    {
        Task<bool> PublishAsync<T>(string topicName, T entity,EventType eventType);
    }
}