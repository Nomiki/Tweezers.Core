using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators
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
            try
            {
                T parsedValue = (T) value;
                return parsedValue != null ? TweezersValidationResult.Accept() : Reject(fieldName);
            }
            catch
            {
                return Reject(fieldName);
            }
        }

        private TweezersValidationResult Reject(string fieldName)
        {
            return TweezersValidationResult.Reject($"Field {fieldName} is not of type {this.Name}");
        }

        public string Name => typeof(T).Name;
    }
}
