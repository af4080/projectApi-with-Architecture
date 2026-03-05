using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace projectApiAngular.Validations
{
    public class StrongPasswordAttribute: ValidationAttribute
    {
        protected  override ValidationResult IsValid(object? value,ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password cannot be null or empty.");
            }
         
            if (password.Length < 7 || password.Length >15)
            {
                return new ValidationResult("Password must be at least 7-15 characters long.");
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }
            return ValidationResult.Success;
        }

    }
}
