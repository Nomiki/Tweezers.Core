using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Schema.DataHolders;

namespace Schema.Validators.Array
{
    public class ArrayValidator : IValidator
    {
        private TweezersValidationMap ValidationMap { get; set; }

        private List<IValidator> Validators { get; set; }

        private ArrayValidator(TweezersValidationMap validationMap)
        {
            this.ValidationMap = validationMap;
            this.Validators = ValidationMap.Compile();
        }

        public static IValidator Create(TweezersValidationMap arrayFieldProperties)
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
                Name = fieldName,
                DisplayName = fieldName,
                FieldProperties = ValidationMap
            };

            return field.Validate(element);
        }

        public string Name => "Array";
    }
}
