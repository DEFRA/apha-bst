﻿namespace Apha.BST.Application.Validation
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }

        public ValidationError(string field, string message, string code)
        {
            Field = field;
            Message = message;
            Code = code;
        }
    }
}
