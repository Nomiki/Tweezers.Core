using Schema.DataHolders;

namespace Schema.Validators.String
{
    public sealed class MaximumLengthValidator : IValidator
    {
        public int Maximum { get; set; }

        private MaximumLengthValidator() { }

        public static MaximumLengthValidator Create(int minimum)
        {
            return new MaximumLengthValidator() {Maximum = minimum};
        }

        public string Name => "Maximum Length";

        public TweezersValidationResult Validate(string fieldName, object value)
        {
            if (value is string)
            {
                string parsedValue = (string)value;
                return parsedValue.Length <= Maximum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"The length of {fieldName} is higher than {Maximum}");
            }

            return TweezersValidationResult.Reject($"Could not parse {fieldName}");
        }
    }
}