using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.Integer
{
    public sealed class MaximumValueValidator : IValidator
    {
        public int Maximum { get; set; }

        private MaximumValueValidator()
        {
        }

        public static MaximumValueValidator Create(int minimum)
        {
            return new MaximumValueValidator() {Maximum = minimum};
        }

        public string Name => "Maximum";

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            try
            {
                int parsedValue = (int) value;
                return parsedValue <= Maximum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"Value of {fieldName} is higher than {Maximum}");
            }
            catch
            {
                return TweezersValidationResult.Reject($"Could not parse {fieldName}");
            }
        }
    }
}