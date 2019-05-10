using System.Linq;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.Password
{
    public sealed class PasswordValidator : IValidator
    {
        private PasswordValidator()
        {

        }

        public static PasswordValidator Create()
        {
            return new PasswordValidator();
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            string passwordStr = value.ToString();
            if (string.IsNullOrWhiteSpace(passwordStr))
            {
                return TweezersValidationResult.Reject("empty password");
            }

            bool hasCapitalLetter = passwordStr.Any(char.IsUpper);
            bool hasLowerLetter = passwordStr.Any(char.IsLower);
            bool hasDigit = passwordStr.Any(char.IsNumber);

            if (hasCapitalLetter && hasLowerLetter && hasDigit)
            {
                return TweezersValidationResult.Accept();
            }

            return TweezersValidationResult.Reject("invalid password, must contain at least one capital letter, one lower letter and one number");
        }

        public string Name => "NotNull";
    }
}