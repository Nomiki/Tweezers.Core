using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;
using Tweezers.Schema.Interfaces;

namespace Tweezers.Schema.DataHolders
{
    public sealed class TweezersObject
    {
        public string CollectionName { get; set; }

        public TweezersDisplayNames DisplayNames { get; } = new TweezersDisplayNames();

        public Dictionary<string, TweezersField> Fields { get; } = new Dictionary<string, TweezersField>();

        public TweezersValidationResult Validate(JObject obj, bool partial)
        {
            JObject filteredObj = obj.Just(Fields.Keys);

            foreach (string fieldName in Fields.Keys)
            {
                TweezersField field = Fields[fieldName];
                dynamic value = filteredObj[fieldName];
                if (value == null && partial)
                {
                    continue;
                }

                TweezersValidationResult validationResultForField = field.Validate(value);
                if (!validationResultForField.Valid)
                {
                    return validationResultForField;
                }
            }

            return TweezersValidationResult.Accept();
        }

        public IEnumerable<JObject> FindInDb(IDatabaseProxy proxy, FindOptions<JObject> findOptions = null)
        {
            FindOptions<JObject> opts = findOptions ?? FindOptions<JObject>.Default();

            return proxy.List(CollectionName, opts).Select(obj => obj.Just(Fields.Keys));
        }

        public dynamic GetById(IDatabaseProxy proxy, string id)
        {
            return proxy.Get(CollectionName, id).Just(Fields.Keys);
        }

        public dynamic Create(IDatabaseProxy proxy, JObject data)
        {
            string id = Guid.NewGuid().ToString();
            JObject filteredData = data.Just(Fields.Keys);
            return proxy.Add(CollectionName, id, filteredData).Just(Fields.Keys);
        }

        public dynamic Update(IDatabaseProxy proxy, string id, JObject data)
        {
            JObject filteredData = data.Just(Fields.Keys);
            return proxy.Edit(CollectionName, id, filteredData).Just(Fields.Keys);
        }

        public bool Delete(IDatabaseProxy proxy, string id)
        {
            return proxy.Delete(CollectionName, id);
        }
    }
}
