using Schema.DataHolders;

namespace Schema.Validators
{
    public sealed class NotNullValidator : IValidator
    {
        private NotNullValidator()
        {

        }

        public static NotNullValidator Create()
        {
            return new NotNullValidator();
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            return value == null
                ? TweezersValidationResult.Reject($"{fieldName} cannot be null.")
                : TweezersValidationResult.Accept();
        }

        public string Name => "NotNull";
    }
}
