using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.Array
{
    public class ArrayValidator : IValidator
    {
        private TweezersFieldProperties FieldProperties { get; set; }

        private ArrayValidator(TweezersFieldProperties fieldProperties)
        {
            this.FieldProperties = fieldProperties;
        }

        public static IValidator Create(TweezersFieldProperties arrayFieldProperties)
        {
            return new ArrayValidator(arrayFieldProperties);
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            if (value is JArray)
            {
                JArray parsedValue = value as JArray;
                int i = 0;
                foreach (var jToken in parsedValue)
                {
                    var element = jToken as JObject;
                    TweezersValidationResult validationResult = ValidateElement($"{fieldName}#{i}", element);
                    if (!validationResult.Valid)
                    {
                        return validationResult;
                    }

                    i++;
                }

                return TweezersValidationResult.Accept();
            }

            return TweezersValidationResult.Reject($"{fieldName} is not an array.");
        }

        private TweezersValidationResult ValidateElement(string fieldName, JObject element)
        {
            TweezersField field = new TweezersField()
            {
                FieldProperties = FieldProperties
            };

            return field.Validate(element);
        }

        public string Name => "Array";
    }
}
