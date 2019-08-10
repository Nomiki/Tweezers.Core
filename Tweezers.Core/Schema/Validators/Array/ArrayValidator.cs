using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

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
            try
            {
                JArray parsedValue = (JArray) value;
                int i = 0;
                foreach (var jToken in parsedValue)
                {
                    TweezersValidationResult validationResult = ValidateElement($"{fieldName}#{i}", jToken);
                    if (!validationResult.Valid)
                    {
                        return validationResult;
                    }

                    i++;
                }

                return TweezersValidationResult.Accept();
            }
            catch (TweezersValidationException e)
            {
                return TweezersValidationResult.Reject(e.Message);
            }
            catch
            {
                return TweezersValidationResult.Reject($"{fieldName} is not an array.");
            }
        }

        private TweezersValidationResult ValidateElement(string fieldName, dynamic element)
        {
            TweezersField field = new TweezersField()
            {
                FieldProperties = FieldProperties
            };
            field.FieldProperties.DisplayName = fieldName;

            return field.Validate(element);
        }

        public string Name => "Array";
    }
}
