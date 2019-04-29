﻿using System;
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

        public string Icon { get; set; }

        public Dictionary<string, TweezersField> Fields { get; set; } = new Dictionary<string, TweezersField>();

        public bool Internal { get; set; } = false;

        public DateTime LastChanged { get; set; } = DateTime.Now;

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

            return proxy.List(CollectionName, opts).Select(obj => obj.Just(AllFields));
        }

        public JObject GetById(IDatabaseProxy proxy, string id)
        {
            return proxy.Get(CollectionName, id).Just(AllFields);
        }

        public JObject Create(IDatabaseProxy proxy, JObject data)
        {
            string id = Guid.NewGuid().ToString();
            JObject filteredData = data.Just(Fields.Keys);
            return proxy.Add(CollectionName, id, filteredData).Just(AllFields);
        }

        public JObject Update(IDatabaseProxy proxy, string id, JObject data)
        {
            JObject filteredData = data.Just(Fields.Keys);
            return proxy.Edit(CollectionName, id, filteredData).Just(AllFields);
        }

        public bool Delete(IDatabaseProxy proxy, string id)
        {
            return proxy.Delete(CollectionName, id);
        }

        private string[] AllFields => 
            EditableFields
                .Concat(new[] {"_id"})
                .ToArray();

        private string[] EditableFields =>
            Fields.Where(f => !f.Value.FieldProperties.UiIgnore)
                .Select(f => f.Key)
                .ToArray();
    }
}
