using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Schema.Validators.Object
{
    class TweezersObjectReferenceValidator : IValidator
    {
        private string ObjectCollectionName { get; set; }
        private TweezersObject ObjectReference { get; set; }

        private TweezersObjectReferenceValidator(string objectCollectionName)
        {
            ObjectCollectionName = objectCollectionName;
            ObjectReference = TweezersSchemaFactory.Find(objectCollectionName, true, true, true);
        }

        public static TweezersObjectReferenceValidator Create(string objectCollectionName)
        {
            return new TweezersObjectReferenceValidator(objectCollectionName);
        }

        public TweezersValidationResult Validate(string fieldName, dynamic value)
        {
            if (ObjectReference == null)
                return TweezersValidationResult.Reject($"Could not find object reference for {ObjectCollectionName}");

            string id = value.ToString();
            JObject obj = ObjectReference.GetById(TweezersSchemaFactory.DatabaseProxy, id);

            return obj == null
                ? TweezersValidationResult.Reject($"Could not find {ObjectReference.SingularName} with id {id}")
                : TweezersValidationResult.Accept();
        }

        public string Name => $"ObjectReference {ObjectReference?.SingularName ?? ObjectCollectionName}";
    }
}