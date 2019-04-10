using System.Text.RegularExpressions;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.String
{
    public sealed class RegexValidator : IValidator
    {
        private string Pattern { get; set; }

        private RegexValidator(string pattern)
        {
            this.Pattern = pattern;
        }

        public static RegexValidator Create(string pattern)
        {
            return new RegexValidator(pattern);
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            if (value is string)
            {
                string parsedValue = (string) value;
                Regex regex = new Regex(Pattern);
                return regex.IsMatch(parsedValue)
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"{fieldName} does not match pattern '{Pattern}'");
            }

            return TweezersValidationResult.Reject($"Could not parse {fieldName}");
        }

        public string Name => "Regex";
    }
}
