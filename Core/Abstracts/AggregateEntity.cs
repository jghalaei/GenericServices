namespace GenericServices.Core.Abstracts;

public abstract class AggregateEntity<TId>
{
    public TId Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
