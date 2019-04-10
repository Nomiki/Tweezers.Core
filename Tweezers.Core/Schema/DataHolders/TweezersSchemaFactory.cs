using System.Collections.Generic;

namespace Schema.DataHolders
{
    public static class TweezersSchemaFactory
    {
        private static Dictionary<string, TweezersObject> tweezersObjects = new Dictionary<string, TweezersObject>();

        public static void AddObject(TweezersObject obj)
        {
            tweezersObjects[obj.CollectionName] = obj;
        }

        public static TweezersObject Find(string collectionName)
        {
            return tweezersObjects.ContainsKey(collectionName) 
                ? tweezersObjects[collectionName] 
                : null;
        }
    }
}
