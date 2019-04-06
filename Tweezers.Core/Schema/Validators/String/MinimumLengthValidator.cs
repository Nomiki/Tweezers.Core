using Schema.DataHolders;

namespace Schema.Validators.String
{
    public sealed class MinimumLengthValidator : IValidator
    {
        public int Minimum { get; set; }

        private MinimumLengthValidator() { }

        public static MinimumLengthValidator Create(int minimum)
        {
            return new MinimumLengthValidator() {Minimum = minimum};
        }

        public string Name => "Minimum Length";

        public TweezersValidationResult Validate(string fieldName, object value)
        {
            if (value is string)
            {
                string parsedValue = (string)value;
                return parsedValue.Length >= Minimum
                    ? TweezersValidationResult.Accept()
                    : TweezersValidationResult.Reject($"The length of {fieldName} is lower than {Minimum}");
            }

            return TweezersValidationResult.Reject($"Could not parse {fieldName}");
        }
    }
}