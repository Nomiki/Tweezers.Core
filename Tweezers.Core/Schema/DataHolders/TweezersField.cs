using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tweezers.Schema.Validators;

namespace Tweezers.Schema.DataHolders
{
    public sealed class TweezersField
    {
        private TweezersFieldProperties _fieldProperties;

        public TweezersFieldProperties FieldProperties
        {
            get => _fieldProperties;
            set
            {
                _fieldProperties = value;
                InternalValidators = _fieldProperties.Compile();
            }
        }

        [JsonIgnore]
        internal List<IValidator> InternalValidators { get; private set; } = new List<IValidator>();

        public TweezersValidationResult Validate(dynamic value)
        {
            if (FieldProperties.Required && value == null)
                return TweezersValidationResult.Reject($"{FieldProperties.DisplayName} is required.");

            foreach (IValidator validator in InternalValidators)
            {
                TweezersValidationResult validationResult = validator.Validate(FieldProperties.DisplayName, value);
                if (!validationResult.Valid)
                {
                    return validationResult;
                }
            }

            return TweezersValidationResult.Accept();
        }

        public void AddExternalValidator(IValidator validator)
        {
            InternalValidators.Add(validator);
        }
    }
}
