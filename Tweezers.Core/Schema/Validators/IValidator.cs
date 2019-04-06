using Schema.DataHolders;

namespace Schema.Validators
{
    public interface IValidator
    {
        TweezersValidationResult Validate(string fieldName, object value);

        string Name { get; }
    }
}
