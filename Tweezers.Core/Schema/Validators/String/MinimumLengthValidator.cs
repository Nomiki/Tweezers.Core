using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.String
{
    public sealed class MinimumLengthValidator : IValidator
    {
        public int Minimum { get; set; }

        private MinimumLengthValidator()
        {
        }

        public static MinimumLengthValidator Create(int minimum)
        {
            return new MinimumLengthValidator() {Minimum = minimum};
        }

        public string Name => "Minimum Length";

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            try
            {
                string parsedValue = (string) value;
                return parsedValue.Length >= Minimum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"The length of {fieldName} is lower than {Minimum}");
            }
            catch
            {
                return TweezersValidationResult.Reject($"Could not parse {fieldName}");
            }
        }
    }
}