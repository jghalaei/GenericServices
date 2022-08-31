using GenericServices.Core.Exceptions;

namespace GenericServices.Core.Services;

public class ServiceValidationResult
{
public bool IsValid { get=> (Errors.Count==0) ;}
public Dictionary<string,string> Errors { get; set; }
public ServiceValidationResult()
{
    Errors=new Dictionary<string, string>();
}
public void ThrowError()
{
    if (Errors.Count>0)
    throw new InvalidEntityException(GenerateMessge());
}

    private string GenerateMessge()
    {
        string str= "";
        foreach(var err in Errors)
        {
            str+= $"{err.Key}: {err.Value}";
        }
        return str;
    }
}