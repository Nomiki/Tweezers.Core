using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Schema.DataHolders
{
    public static class TweezersSchemaFactory
    {
        private static readonly string ObjectMetadataCollectionName = "tweezers-objects";

        public static IDatabaseProxy DatabaseProxy { get; set; }

        public static void AddObject(TweezersObject obj)
        {
            TweezersObject dbObj = InternalFind(obj.CollectionName, true);
            if (dbObj?.Internal ?? false)
                throw new ArgumentException("trying to override an internal object");

            DatabaseProxy.Add(ObjectMetadataCollectionName, obj.CollectionName, JObject.FromObject(obj));
        }

        private static TweezersObject InternalFind(string collectionName, bool withInternalObjects = false, bool withInternalFields = false)
        {
            JObject tweezersDbJObject = DatabaseProxy.Get(ObjectMetadataCollectionName, collectionName);
            TweezersObject dbObj = tweezersDbJObject.ToStrongType<TweezersObject>();
            if (dbObj != null)
            {
                TweezersObject clone = JObject.FromObject(dbObj).ToStrongType<TweezersObject>();
                if (clone.Internal && !withInternalObjects)
                {
                    return null;
                }

                if (clone.Internal)
                {
                    clone.Fields = clone.Fields.Where(f => withInternalFields || !f.Value.FieldProperties.UiIgnore)
                        .ToDictionary(f => f.Key, f => f.Value);
                }

                return clone;
            }

            return null;
        }

        public static TweezersObject Find(string collectionName, bool withInternalObjects = false, bool withInternalFields = false)
        {
            TweezersObject obj = InternalFind(collectionName, withInternalObjects, withInternalFields);
            if (obj == null)
                throw new TweezersValidationException(
                    TweezersValidationResult.Reject($"Could not find collection with name {collectionName}"));

            return obj;
        }

        public static IEnumerable<TweezersObject> GetAll(bool includeInternal = false)
        {
            return DatabaseProxy.List(ObjectMetadataCollectionName, FindOptions<JObject>.Default(0, 50))
                .Select(jObj => jObj.ToStrongType<TweezersObject>())
                .Where(tweezersObj => includeInternal || !tweezersObj.Internal);
        }

        public static bool DeleteObject(string collectionName)
        {
            return DatabaseProxy.Delete(ObjectMetadataCollectionName, collectionName);
        }
    }
}