using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Schema.Common;

namespace Schema.DataHolders
{
    public sealed class TweezersObject
    {
        public string CollectionName { get; set; }

        public TweezersDisplayNames DisplayNames { get; } = new TweezersDisplayNames();

        public Dictionary<string, TweezersField> Fields { get; } = new Dictionary<string, TweezersField>();

        public TweezersValidationResult Validate(JObject obj)
        {
            JObject filteredObj = obj.Just(Fields.Keys);

            foreach (string fieldName in Fields.Keys)
            {
                TweezersField field = Fields[fieldName];
                TweezersValidationResult validationResultForField = field.Validate(filteredObj[fieldName]);
                if (!validationResultForField.Valid)
                {
                    return validationResultForField;
                }
            }

            return TweezersValidationResult.Accept();
        }
    }
}
