using System.Collections.Generic;

namespace Tweezers.Schema.DataHolders
{
    public static class TweezersSchemaFactory
    {
        private static readonly Dictionary<string, TweezersObject> TweezersObjects = new Dictionary<string, TweezersObject>();

        public static void AddObject(TweezersObject obj)
        {
            TweezersObjects[obj.CollectionName] = obj;
        }

        public static TweezersObject Find(string collectionName)
        {
            return TweezersObjects.ContainsKey(collectionName) 
                ? TweezersObjects[collectionName] 
                : null;
        }
    }
}
