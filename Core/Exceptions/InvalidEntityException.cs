namespace GenericServices.Core.Exceptions;

public class InvalidEntityException : HttpResponseException
{
    public InvalidEntityException(string message, object? value = null) : base(message, 400, value)
    {
    }
}


