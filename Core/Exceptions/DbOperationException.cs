namespace GenericServices.Core.Exceptions;

public class DbOperationException : HttpResponseException
{
    public DbOperationException(string message, object? value = null) : base(message, 500, value)
    {
    }
}

public class DbInsertException : DbOperationException
{
    public DbInsertException(string message, object? value = null) : base(message, value)
    {
    }
}

public class DbUpdateException : DbOperationException
{
    public DbUpdateException(string message, object? value = null) : base(message, value)
    {
    }
}

public class DbDelteException : DbOperationException
{
    public DbDelteException(string message, object? value = null) : base(message, value)
    {
    }
}

public class DbSelectException : DbOperationException
{
    public DbSelectException(string message, object? value = null) : base(message, value)
    {
    }
}