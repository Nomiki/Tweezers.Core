using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.Validators;
using Tweezers.Schema.Validators.Array;
using Tweezers.Schema.Validators.Integer;
using Tweezers.Schema.Validators.Object;
using Tweezers.Schema.Validators.Password;
using Tweezers.Schema.Validators.String;

// ReSharper disable PossibleInvalidOperationException
namespace Tweezers.Schema.DataHolders
{
    public class TweezersFieldProperties
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public TweezersFieldType FieldType { get; set; }

        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Regex { get; set; }

        public string NumericSuffix { get; set; }

        public string[] PossibleValues { get; set; }

        public bool Required { get; set; } = false;

        public string ObjectName { get; set; }

        public TweezersObject ObjectReference { get; set; }

        public bool UiIgnore { get; set; }

        public bool GridIgnore { get; set; }

        public bool UiTitle { get; set; }

        public TweezersFieldProperties ArrayFieldProperties { get; set; }

        public int OrderNum { get; set; } = 1024;

        public List<IValidator> Compile()
        {
            List<IValidator> validators = new List<IValidator>();
            if (Required)
            {
                validators.Add(NotNullValidator.Create());
            }

            validators.Add(FieldTypeValidator);

            if (Min.HasValue)
            {
                validators.Add(MinValidator);
            }

            if (Max.HasValue)
            {
                validators.Add(MaxValidator);
            }

            AddRegexValidatorIfNeeded(validators);

            AddEnumValidatorIfNeeded(validators);

            AddObjectValidatorIfNeeded(validators);

            AddArrayValidatorIfNeeded(validators);

            AddPasswordValidatorIfNeeded(validators);

            AddTagsValidatorIfNeeded(validators);

            return validators;
        }

        private void AddTagsValidatorIfNeeded(List<IValidator> validators)
        {
            if (FieldType.Equals(TweezersFieldType.TagsArray))
            {
                TweezersFieldProperties tagsFieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                    Min = 1,
                    Max = 50,
                    Regex = $"^[A-Za-z\\d ]+$",
                };

                validators.Add(ArrayValidator.Create(tagsFieldProperties));
            }
        }

        private void AddPasswordValidatorIfNeeded(List<IValidator> validators)
        {
            if (FieldType.Equals(TweezersFieldType.Password))
            {
                validators.Add(PasswordValidator.Create());
            }
        }

        private void AddArrayValidatorIfNeeded(List<IValidator> validators)
        {
            if (FieldType.Equals(TweezersFieldType.Array))
            {
                if (this.ArrayFieldProperties != null)
                {
                    validators.Add(ArrayValidator.Create(ArrayFieldProperties));
                }
                else
                {
                    throw new ArgumentException($"ArrayFieldProperties are required for 'array' type");
                }
            }
        }

        private void AddObjectValidatorIfNeeded(List<IValidator> validators)
        {
            if (this.FieldType.Equals(TweezersFieldType.Object))
            {
                if (ObjectReference != null)
                {
                    validators.Add(TweezersObjectValidator.Create(ObjectReference));
                }
                else if (ObjectName != null)
                {
                    TweezersObject objectReference = TweezersSchemaFactory.Find(ObjectName, true, true, true);
                    if (objectReference == null)
                    {
                        throw new ArgumentException($"Could not find Tweezers Object {ObjectName}");
                    }

                    validators.Add(TweezersObjectReferenceValidator.Create(ObjectName));
                }
                else
                {
                    throw new ArgumentException("ObjectName or JSON is required for 'object' type.");
                }
            }
        }

        private void AddEnumValidatorIfNeeded(List<IValidator> validators)
        {
            if (this.FieldType.Equals(TweezersFieldType.Enum))
            {
                validators.Add(ValuesValidator.Create(PossibleValues));
            }
        }

        private void AddRegexValidatorIfNeeded(List<IValidator> validators)
        {
            if (this.FieldType.Equals(TweezersFieldType.String) && Regex != null)
            {
                validators.Add(RegexValidator.Create(Regex));
            }
        }

        private IValidator FieldTypeValidator
        {
            get
            {
                switch (this.FieldType)
                {
                    case TweezersFieldType.Integer:
                        return TypeValidator<int>.Create();
                    case TweezersFieldType.String:
                    case TweezersFieldType.Enum:
                    case TweezersFieldType.Password:
                        return TypeValidator<string>.Create();
                    case TweezersFieldType.Boolean:
                        return TypeValidator<bool>.Create();
                    case TweezersFieldType.Object:
                        return ObjectName != null ? (IValidator)TypeValidator<string>.Create() : TypeValidator<JObject>.Create();
                    case TweezersFieldType.Array:
                        return TypeValidator<JArray>.Create();
                    case TweezersFieldType.TagsArray:
                        return TypeValidator<JArray>.Create();
                    default:
                        throw new NotImplementedException($"Unsupported field type {FieldType}");
                }
            }
        }

        private IValidator MinValidator
        {
            get
            {
                switch (this.FieldType)
                {
                    case TweezersFieldType.Integer:
                        return MinimumValueValidator.Create(Min.Value);
                    case TweezersFieldType.String:
                    case TweezersFieldType.Password:
                        return MinimumLengthValidator.Create(Min.Value);
                    default:
                        throw new ArgumentException($"Min is not supported for type {this.FieldType.ToString()}");
                }
            }
        }

        private IValidator MaxValidator
        {
            get
            {
                switch (this.FieldType)
                {
                    case TweezersFieldType.Integer:
                        return MaximumValueValidator.Create(Max.Value);
                    case TweezersFieldType.String:
                    case TweezersFieldType.Password:
                        return MaximumLengthValidator.Create(Max.Value);
                    default:
                        throw new ArgumentException($"Min is not supported for type {this.FieldType.ToString()}");
                }
            }
        }
    }
}