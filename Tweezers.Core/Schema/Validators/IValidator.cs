using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators
{
    public interface IValidator
    {
        TweezersValidationResult Validate(string fieldName, dynamic value);

        string Name { get; }
    }
}
