using Schema.DataHolders;

namespace Schema.Validators
{
    public sealed class TypeValidator<T> : IValidator
    {
        private TypeValidator()
        {

        }

        public static TypeValidator<T> Create()
        {
            return new TypeValidator<T>();
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            return (value is T)
                ? TweezersValidationResult.Accept()
                : TweezersValidationResult.Reject($"Field {fieldName} is not of type {this.Name}");
        }

        public string Name => typeof(T).Name;
    }
}
