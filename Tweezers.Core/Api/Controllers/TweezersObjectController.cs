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

        /// <summary>
        /// Query a list of TweezersObject metadata
        /// </summary>
        /// <param name="skip">How many objects to skip</param>
        /// <param name="take">How many objects to take</param>
        /// <param name="sortField">Sort the results by this field</param>
        /// <param name="direction">'asc' or 'desc'</param>
        /// <param name="internalObj">Determine if the query should fetch internal objects</param>
        /// <returns>A list of TweezersObjects, sorted and paginated</returns>
        [HttpGet("tweezers-schema")]
        public ActionResult<TweezersMultipleResults<TweezersObject>> List([FromQuery] bool internalObj,
            [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            return WrapWithAuthorizationCheck(() =>
            {
                IEnumerable<TweezersObject> allMetadata = TweezersSchemaFactory.GetAll(includeInternal: internalObj);

                if (SchemaManagement.CanChangeSchema && internalObj)
                {
                    allMetadata = allMetadata.Concat(new[] {SchemaManagement.SchemaMetadata});
                }

                allMetadata = allMetadata
                    .Skip(skip)
                    .Take(take)
                    .ToArray();

                string sortFieldInternal = string.IsNullOrWhiteSpace(sortField) ? "collectionName" : sortField;
                IEnumerable <TweezersObject> sortedMetadata = direction == "asc"
                    ? allMetadata.OrderBy(m => JObject.FromObject(m, Serializer.JsonSerializer)[sortFieldInternal].ToString())
                    : allMetadata.OrderByDescending(m => JObject.FromObject(m, Serializer.JsonSerializer)[sortFieldInternal].ToString());

                return TweezersOk(TweezersMultipleResults<TweezersObject>.Create(sortedMetadata, allMetadata.Count()));
            }, "List", DefaultPermission.None, TweezersSchemaKey);
        }

        /// <summary>
        /// Query a specific TweezersObject metadata from the DB
        /// </summary>
        /// <param name="collectionName">Desired metadata collection name</param>
        /// <param name="internalObj">Determine if the query should search it in the internal objects as well</param>
        /// <returns>The desired TweezersObject for the collection</returns>
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

                    objectMetadata.Fields = objectMetadata.Fields.OrderBy(f => f.Value.FieldProperties.OrderNum)
                        .ToDictionary(f => f.Key, f => f.Value);

                    return TweezersOk(objectMetadata);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Get", DefaultPermission.None, TweezersSchemaKey);
        }

        /// <summary>
        /// Creates a new TweezersObject metadata
        /// </summary>
        /// <param name="data">Created TweezersObject data</param>
        /// <returns>The newly created TweezersObject</returns>
        [HttpPost("tweezers-schema")]
        public ActionResult<TweezersObject> Post([FromBody] TweezersObject data)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                try
                {
                    TweezersObject obj = ReplaceTweezersObject(data);
                    IdentityManager.AppendNewPermission(obj);
                    SchemaManagement.AddObjectReference(obj.CollectionName);
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

        /// <summary>
        /// Modifies a TweezersObject metadata
        /// </summary>
        /// <param name="collectionName">Collection name of the modified TweezersObject</param>
        /// <param name="data">Change set</param>
        /// <returns>The changed TweezersObject</returns>
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

        /// <summary>
        /// Deletes a TweezersObject from the collection
        /// </summary>
        /// <param name="collectionName">TweezersObject collection name to be deleted</param>
        /// <returns>200 if the TweezersObject was deleted</returns>
        [HttpDelete("tweezers-schema/{collectionName}")]
        public ActionResult<bool> Delete(string collectionName)
        {
            return WrapWithAuthorizationCheck(() =>
            {
                bool deleted = TweezersSchemaFactory.DeleteObject(collectionName);
                IdentityManager.DeletePermission(collectionName);
                SchemaManagement.RemoveObjectReference(collectionName);
                return TweezersOk();
            }, "Delete", DefaultPermission.Edit, TweezersSchemaKey);
        }
    }
}