using System.Text.RegularExpressions;
using Schema.DataHolders;

namespace Schema.Validators.String
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

        public TweezersValidationResult Validate(string fieldName, object value)
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
