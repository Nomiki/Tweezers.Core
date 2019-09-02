using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Controllers;
using Tweezers.Api.Identity.Managers;
using Tweezers.DBConnector;

namespace Tweezers.Api.Identity.Controllers
{
    [Route("api")]
    [ApiController]
    public class RolesController : TweezersControllerBase
    {
        protected override bool WithInternalObjects => true;
        protected string CollectionName => IdentityManager.RolesSchemaName;

        [HttpGet("tweezers-roles")]
        public ActionResult<TweezersMultipleResults<JObject>> List([FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            return base.List(CollectionName, skip, take, sortField, direction);
        }

        [HttpGet("tweezers-roles/{id}")]
        public ActionResult<JObject> Get(string id)
        {
            return base.Get(CollectionName, id);
        }

        [HttpPost("tweezers-roles")]
        public ActionResult<JObject> Post([FromBody] JObject data)
        {
            return base.Post(CollectionName, data, data["name"]?.ToString());
        }

        [HttpPatch("tweezers-roles/{id}")]
        public ActionResult<JObject> Patch(string id, [FromBody] JObject data)
        {
            return base.Patch(CollectionName, id, data);
        }

        [HttpDelete("tweezers-roles/{id}")]
        public ActionResult<JObject> Delete(string id)
        {
            return base.Delete(CollectionName, id);
        }
    }
}
