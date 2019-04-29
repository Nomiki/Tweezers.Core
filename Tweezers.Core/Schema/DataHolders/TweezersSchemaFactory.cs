using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;
using Tweezers.Schema.Interfaces;

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

        private static TweezersObject InternalFind(string collectionName, bool withInternalObjects = false)
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
                    clone.Fields = clone.Fields.Where(f => !f.Value.FieldProperties.UiIgnore)
                        .ToDictionary(f => f.Key, f => f.Value);
                }

                return clone;
            }

            return null;
        }

        public static TweezersObject Find(string collectionName, bool withInternalObjects = false)
        {
            TweezersObject obj = InternalFind(collectionName, withInternalObjects);
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