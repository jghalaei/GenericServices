using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericServices.Core.Entities
{
    public record MessagingDataWrapper<T>
    {
        public MessagingDataWrapper(T entity, EventType eventType)
        {
            EventType = eventType;
            Entity = entity;

        }
        public T Entity { get; set; }
        public EventType EventType { get; set; }

    }
}