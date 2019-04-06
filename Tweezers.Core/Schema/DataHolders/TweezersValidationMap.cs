using System;
using System.Collections.Generic;
using Schema.Validators;
using Schema.Validators.Integer;
using Schema.Validators.String;

// ReSharper disable PossibleInvalidOperationException
namespace Schema.DataHolders
{
    public class TweezersValidationMap
    {
        public TweezersFieldType FieldType { get; set; }

        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Regex { get; set; }

        public string[] Values { get; set; }

        public bool Required { get; set; } = false;

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

            if (this.FieldType.Equals(TweezersFieldType.String) && Regex != null)
            {
                validators.Add(RegexValidator.Create(Regex));
            }

            if (this.FieldType.Equals(TweezersFieldType.Enum))
            {
                if (Values.Length == 0)
                {
                    throw new ArgumentException($"Enum field detected without any defined values.");
                }

                validators.Add(ValuesValidator.Create(Values));
            }

            return validators;
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
                        return TypeValidator<string>.Create();
                    case TweezersFieldType.Boolean:
                        return TypeValidator<bool>.Create();
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
                        return MaximumLengthValidator.Create(Max.Value);
                    default:
                        throw new ArgumentException($"Min is not supported for type {this.FieldType.ToString()}");
                }
            }
        }
    }
}