namespace Apha.BST.Application.Validation
{
 public class BusinessValidationErrorException : Exception
{
    public string Status { get; set; }
    public string ExceptionMessage { get; set; }
    public List<BusinessValidationError> Errors { get; set; }

    public BusinessValidationErrorException(List<BusinessValidationError> errors)
    {
        Status = "error";
        ExceptionMessage = "Business validation failed.";
        Errors = errors;
    }
}
}
