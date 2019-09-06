using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Identity.Managers;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public sealed class TweezersObjectController : TweezersControllerBase
    {
        private const string TweezersSchemaKey = "tweezers-schema";

        [HttpGet("tweezers-schema")]
        public ActionResult<TweezersMultipleResults<TweezersObject>> List([FromQuery] bool internalObj)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                IEnumerable<TweezersObject> allMetadata = TweezersSchemaFactory.GetAll(includeInternal: internalObj);

                if (SchemaManagement.CanChangeSchema && internalObj)
                {
                    allMetadata = allMetadata.Concat(new[] {SchemaManagement.SchemaMetadata});
                }

                return TweezersOk(TweezersMultipleResults<TweezersObject>.Create(allMetadata));
            }, "List", DefaultPermission.None, TweezersSchemaKey);
        }

        [HttpGet("tweezers-schema/{collectionName}")]
        public ActionResult<TweezersObject> Get(string collectionName, [FromQuery] bool internalObj)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                if (collectionName == TweezersSchemaKey && SchemaManagement.CanChangeSchema)
                {
                    return TweezersOk(SchemaManagement.SchemaMetadata);
                }

                try
                {
                    TweezersObject objectMetadata =
                        TweezersSchemaFactory.Find(collectionName, withInternalObjects: internalObj);

                    return TweezersOk(objectMetadata);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Get", DefaultPermission.None, TweezersSchemaKey);
        }

        [HttpPost("tweezers-schema")]
        public ActionResult<TweezersObject> Post([FromBody] TweezersObject data)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                try
                {
                    TweezersObject obj = ReplaceTweezersObject(data);
                    IdentityManager.AppendNewPermission(obj);
                    return TweezersCreated(obj);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Post", DefaultPermission.Edit, TweezersSchemaKey);
        }

        private static TweezersObject ReplaceTweezersObject(TweezersObject data)
        {
            data.Fields = data.Fields.ToDictionary(kvp => kvp.Value.FieldProperties.Name, kvp => kvp.Value);

            JObject dataAsJObject = JObject.FromObject(data, Serializer.JsonSerializer);
            dataAsJObject["fields"] = JArray.FromObject(
                data.Fields.Select(kvp => JObject.FromObject(kvp.Value.FieldProperties, Serializer.JsonSerializer)));

            TweezersValidationResult validationResult =
                SchemaManagement.SchemaMetadata.Validate(dataAsJObject, false);

            TweezersSchemaFactory.AddObject(data);
            TweezersObject obj = TweezersSchemaFactory.Find(data.CollectionName);
            return obj;
        }

        [HttpPatch("tweezers-schema/{collectionName}")]
        public ActionResult<TweezersObject> Patch(string collectionName, [FromBody] TweezersObject data)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                if (collectionName == TweezersSchemaKey)
                    return TweezersNotFound();

                try
                {
                    data.CollectionName = collectionName;
                    TweezersObject obj = ReplaceTweezersObject(data);
                    IdentityManager.EditPermissionName(obj);
                    return TweezersOk(obj);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Patch", DefaultPermission.Edit, TweezersSchemaKey);
        }

        [HttpDelete("tweezers-schema/{collectionName}")]
        public ActionResult<bool> Delete(string collectionName)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                bool deleted = TweezersSchemaFactory.DeleteObject(collectionName);
                IdentityManager.DeletePermission(collectionName);
                return TweezersOk();
            }, "Delete", DefaultPermission.Edit, TweezersSchemaKey);
        }
    }
}