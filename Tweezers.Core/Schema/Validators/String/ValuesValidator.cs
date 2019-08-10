using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.String
{
    public sealed class ValuesValidator : IValidator
    {
        private string[] AllowedValues { get; set; }

        private ValuesValidator(string[] allowedValues)
        {
            this.AllowedValues = allowedValues;
        }

        public static ValuesValidator Create(string[] allowedValues)
        {
            return new ValuesValidator(allowedValues);
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            try
            {
                string parsedValue = (string) value;
                return parsedValue.In(AllowedValues)
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject(
                        $"Invalid {fieldName}, possible values are: [{AllowedValues.ToArrayString()}]");
            }
            catch
            {
                return TweezersValidationResult.Reject($"Could not parse {fieldName}");
            }
        }

        public string Name => "Values";
    }
}
