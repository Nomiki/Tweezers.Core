using System.Linq;
using Tweezers.Api.Utils;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Schema
{
    public static class SchemaManagement
    {
        public static TweezersObject SchemaMetadata { get; }

        public static bool CanChangeSchema { get; set; }

        static SchemaManagement()
        {
            SchemaMetadata = Schemas.SchemaMetaJson.Deserialize<TweezersObject>();
            PopulateObjectReferenceArray();
        }

        private static TweezersFieldProperties ObjectNameFieldProperties
        {
            get => SchemaMetadata.Fields["fields"].FieldProperties.ArrayFieldProperties.ObjectReference
                .Fields["objectName"].FieldProperties;
            set => SchemaMetadata.Fields["fields"].FieldProperties.ArrayFieldProperties.ObjectReference
                .Fields["objectName"].FieldProperties = value;
        }

        private static string[] ObjectNamePossibleValues
        {
            get => ObjectNameFieldProperties.PossibleValues;
            set
            {
                ObjectNameFieldProperties.PossibleValues = value;
                ObjectNameFieldProperties = ObjectNameFieldProperties;
            }
        }

        private static void PopulateObjectReferenceArray()
        {
            ObjectNamePossibleValues = TweezersSchemaFactory.GetAll().Select(obj => obj.CollectionName).ToArray();
        }

        public static void AddObjectReference(string name)
        {
            ObjectNamePossibleValues = ObjectNamePossibleValues
                    .Append(name)
                    .Distinct()
                    .ToArray();
        }

        public static void RemoveObjectReference(string name)
        {
            ObjectNamePossibleValues = ObjectNamePossibleValues
                    .Where(obj => obj != name)
                    .Distinct()
                    .ToArray();
        }
    }
}
