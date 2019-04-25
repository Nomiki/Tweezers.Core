using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TweezersObjectController : TweezersControllerBase
    {
        [HttpGet("tweezers-schema")]
        public virtual ActionResult<IEnumerable<TweezersObject>> List([FromQuery] bool internalObj)
        {
            return new ActionResult<IEnumerable<TweezersObject>>(TweezersSchemaFactory.GetAll(includeInternal: internalObj));
        }

        [HttpGet("tweezers-schema/{collectionName}")]
        public virtual ActionResult<TweezersObject> Get(string collectionName, [FromQuery] bool internalObj)
        {
            return new ActionResult<TweezersObject>(TweezersSchemaFactory.Find(collectionName, withInternalObjects: internalObj));
        }

        [HttpPost("tweezers-schema")]
        public virtual ActionResult<TweezersObject> Post([FromBody] TweezersObject data)
        {
            return OverrideObject(data);
        }

        private static ActionResult<TweezersObject> OverrideObject(TweezersObject data)
        {
            TweezersSchemaFactory.AddObject(data);
            return new ActionResult<TweezersObject>(TweezersSchemaFactory.Find(data.CollectionName));
        }

        [HttpPut("tweezers-schema/{collectionName}")]
        public virtual ActionResult<TweezersObject> Patch(string collectionName, [FromBody] TweezersObject data)
        {
            data.CollectionName = collectionName;
            return OverrideObject(data);
        }

        [HttpDelete("tweezers-schema/{collectionName}")]
        public virtual ActionResult<bool> Delete(string collectionName)
        {
            return new ActionResult<bool>(TweezersSchemaFactory.DeleteObject(collectionName));
        }
    }
}
