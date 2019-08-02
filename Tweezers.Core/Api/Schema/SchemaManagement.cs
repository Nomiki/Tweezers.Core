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
        }
    }
}
