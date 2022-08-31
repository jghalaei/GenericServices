namespace GenericServices.Core.Exceptions;

public class HttpResponseException : Exception
{
    public int StatusCode{get;}
    public object? Value {get;}
    public HttpResponseException(string message,int statusCode, object? value=null) : base(message)
    {
        StatusCode=statusCode;
        Value=value;
    }
    
}
