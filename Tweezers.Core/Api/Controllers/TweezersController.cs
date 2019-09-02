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
        public new virtual ActionResult<TweezersMultipleResults<JObject>> List(string collection, 
            [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            return base.List(collection, skip, take, sortField, direction);
        }

        [HttpGet("{collection}/{id}")]
        public new virtual ActionResult<JObject> Get(string collection, string id)
        {
            return base.Get(collection, id);
        }

        [HttpPost("{collection}")]
        public new virtual ActionResult<JObject> Post(string collection, [FromBody] JObject data)
        {
            return base.Post(collection, data);
        }

        [HttpPatch("{collection}/{id}")]
        public new virtual ActionResult<JObject> Patch(string collection, string id, [FromBody] JObject data)
        {
            return base.Patch(collection, id, data);
        }

        [HttpDelete("{collection}/{id}")]
        public new virtual ActionResult<JObject> Delete(string collection, string id)
        {
            return base.Delete(collection, id);
        }
    }
}