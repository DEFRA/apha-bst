using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Apha.BST.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CustomEmailAttribute : ValidationAttribute
    {
        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        public CustomEmailAttribute()
        {
            ErrorMessage = "Please enter a valid email address";
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
                return true; // Allow nulls — [Required] should be used for non-null validation

            if (value is not string email)
                return false; // Reject non-string inputs

            if (string.IsNullOrWhiteSpace(email))
                return true; // Let [Required] handle empty values

            return EmailRegex.IsMatch(email);
        }
    }
}
