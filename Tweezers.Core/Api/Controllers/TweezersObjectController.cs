using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public sealed class TweezersObjectController : TweezersControllerBase
    { 
        [HttpGet("tweezers-schema")]
        public ActionResult<TweezersMultipleResults<TweezersObject>> List([FromQuery] bool internalObj)
        {
            if (!IsSessionValid())
                return TweezersUnauthorized();

            IEnumerable<TweezersObject> allMetadata = TweezersSchemaFactory.GetAll(includeInternal: internalObj);
            return TweezersOk(TweezersMultipleResults<TweezersObject>.Create(allMetadata));
        }

        [HttpGet("tweezers-schema/{collectionName}")]
        public ActionResult<TweezersObject> Get(string collectionName, [FromQuery] bool internalObj)
        {
            if (!IsSessionValid())
                return TweezersUnauthorized();

            TweezersObject objectMetadata = TweezersSchemaFactory.Find(collectionName, withInternalObjects: internalObj);
            return TweezersOk(objectMetadata);
        }

        [HttpPost("tweezers-schema")]
        public ActionResult<TweezersObject> Post([FromBody] TweezersObject data)
        {
            if (!IsSessionValid())
                return TweezersUnauthorized();

            TweezersObject obj = ReplaceTweezersObject(data);
            return TweezersCreated(obj);
        }

        private static TweezersObject ReplaceTweezersObject(TweezersObject data)
        {
            TweezersSchemaFactory.AddObject(data);
            // todo: add validate
            TweezersObject obj = TweezersSchemaFactory.Find(data.CollectionName);
            return obj;
        }

        [HttpPut("tweezers-schema/{collectionName}")]
        public ActionResult<TweezersObject> Patch(string collectionName, [FromBody] TweezersObject data)
        {
            if (!IsSessionValid())
                return TweezersUnauthorized();

            data.CollectionName = collectionName;
            TweezersObject obj = ReplaceTweezersObject(data);
            return TweezersOk(obj);
        }

        [HttpDelete("tweezers-schema/{collectionName}")]
        public ActionResult<bool> Delete(string collectionName)
        {
            if (!IsSessionValid())
                return TweezersUnauthorized();

            bool deleted = TweezersSchemaFactory.DeleteObject(collectionName);
            return TweezersOk(deleted);
        }
    }
}
