using Schema.DataHolders;

namespace Schema.Validators
{
    public interface IValidator
    {
        TweezersValidationResult Validate(string fieldName, dynamic value);

        string Name { get; }
    }
}
