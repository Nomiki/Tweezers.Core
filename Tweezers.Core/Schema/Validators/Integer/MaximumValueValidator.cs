using Schema.DataHolders;

namespace Schema.Validators.Integer
{
    public sealed class MaximumValueValidator : IValidator
    {
        public int Maximum { get; set; }

        private MaximumValueValidator() { }

        public static MaximumValueValidator Create(int minimum)
        {
            return new MaximumValueValidator() {Maximum = minimum};
        }

        public string Name => "Maximum";

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            if (value is int)
            {
                int parsedValue = (int)value;
                return parsedValue <= Maximum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"Value of {fieldName} is higher than {Maximum}");
            }

            return TweezersValidationResult.Reject($"Could not parse {fieldName}");
        }
    }
}