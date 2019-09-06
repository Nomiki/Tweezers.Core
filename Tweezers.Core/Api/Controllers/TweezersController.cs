using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TweezersController : TweezersControllerBase
    {
        [HttpGet("{collection}")]
        public virtual ActionResult<TweezersMultipleResults<JObject>> List(string collection, 
            [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            return base.List(collection, skip, take, sortField, direction);
        }

        [HttpGet("{collection}/{id}")]
        public virtual ActionResult<JObject> Get(string collection, string id)
        {
            return base.Get(collection, id);
        }

        [HttpPost("{collection}")]
        public virtual ActionResult<JObject> Post(string collection, [FromBody] JObject data, string suggestedId = null)
        {
            return base.Post(collection, data, suggestedId);
        }

        [HttpPatch("{collection}/{id}")]
        public virtual ActionResult<JObject> Patch(string collection, string id, [FromBody] JObject data)
        {
            return base.Patch(collection, id, data);
        }

        [HttpDelete("{collection}/{id}")]
        public virtual ActionResult<JObject> Delete(string collection, string id)
        {
            return base.Delete(collection, id);
        }
    }
}