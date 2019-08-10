using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.Integer
{
    public sealed class MinimumValueValidator : IValidator
    {
        public int Minimum { get; set; }

        private MinimumValueValidator()
        {
        }

        public static MinimumValueValidator Create(int minimum)
        {
            return new MinimumValueValidator() {Minimum = minimum};
        }

        public string Name => "Minimum";

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            try
            {
                int parsedValue = (int) value;
                return parsedValue >= Minimum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"Value of {fieldName} is lower than {Minimum}");
            }
            catch
            {
                return TweezersValidationResult.Reject($"Could not parse {fieldName}");
            }
        }
    }
}