using System.Collections.Generic;
using Tweezers.Schema.DataHolders.Exceptions;
using Tweezers.Schema.Interfaces;

namespace Tweezers.Schema.DataHolders
{
    public static class TweezersSchemaFactory
    {
        private static readonly Dictionary<string, TweezersObject> TweezersObjects = new Dictionary<string, TweezersObject>();
        public static IDatabaseProxy DatabaseProxy { get; set; }

        public static void AddObject(TweezersObject obj)
        {
            TweezersObjects[obj.CollectionName] = obj;
        }

        private static TweezersObject InternalFind(string collectionName, bool withInternalObjects = false)
        {
            if (TweezersObjects.ContainsKey(collectionName))
            {
                TweezersObject tweezersObject = TweezersObjects[collectionName];
                return tweezersObject.Internal && !withInternalObjects ? null : tweezersObject;
            }

            return null;
        }

        public static TweezersObject Find(string collectionName, bool withInternalObjects = false)
        {
            TweezersObject obj = InternalFind(collectionName, withInternalObjects);
            if (obj == null) 
                throw new TweezersValidationException(TweezersValidationResult.Reject($"Could not find collection with name {collectionName}"));

            return obj;
        }
    }
}
