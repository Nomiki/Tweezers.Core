using System.Collections.Generic;
using Tweezers.Schema.DataHolders.Exceptions;
using Tweezers.Schema.Interfaces;

namespace Tweezers.Schema.DataHolders
{
    public static class TweezersSchemaFactory
    {
        private static readonly Dictionary<string, TweezersObject> TweezersObjects = new Dictionary<string, TweezersObject>();

        public static void AddObject(TweezersObject obj)
        {
            TweezersObjects[obj.CollectionName] = obj;
        }

        private static TweezersObject InternalFind(string collectionName)
        {
            return TweezersObjects.ContainsKey(collectionName) 
                ? TweezersObjects[collectionName] 
                : null;
        }

        public static TweezersObject Find(string collectionName)
        {
            TweezersObject obj = InternalFind(collectionName);
            if (obj == null) 
                throw new TweezersValidationException(TweezersValidationResult.Reject($"Could not find collection with name {collectionName}"));

            return obj;
        }

        public static IDatabaseProxy DatabaseProxy { get; set; }
    }
}
