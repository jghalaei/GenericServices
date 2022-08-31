namespace GenericServices.Core.Exceptions;

public class EntityNotFoundException : HttpResponseException
{
    public EntityNotFoundException(string message, object? value = null) : base(message, 404, value)
    {
    }
}
