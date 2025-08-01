namespace Apha.BST.Application.Validation
{
    public class BusinessValidationError
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public object Details { get; set; }

        public BusinessValidationError(string message, string code, object details)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }
}
