namespace Apha.BST.Application.Validation
{
    public class ValidationErrorResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ValidationError> Errors { get; set; }

        public ValidationErrorResponse(List<ValidationError> errors)
        {
            Status="error";
            Message = "Validation failed.";
            Errors = errors;
        }
    }
}
