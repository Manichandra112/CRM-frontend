namespace CRM_Backend.Exceptions;

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message)
        : base(message, StatusCodes.Status422UnprocessableEntity)
    {
    }
}
