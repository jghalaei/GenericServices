using System.Collections;

namespace GenericServices.Core.Abstracts;

public abstract class AggregateRoot<T, TId> : AggregateEntity<TId>
{
    public void UpdateValues(T source)
    {
        var type = typeof(T);
        foreach (var prop in type.GetProperties())
        {
            prop.SetValue(this, prop.GetValue(source));
        }
    }
    public List<TEntity> GetEntities<TEntity>()
    {
        var type = this.GetType();
        foreach(var prop in type.GetProperties())
        {
            if (prop.PropertyType== typeof(List<TEntity>) )
            return  prop.GetValue(this) as List<TEntity>;
        }

            return null;

    }
}