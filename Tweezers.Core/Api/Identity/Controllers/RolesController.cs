using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Controllers;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Identity.Managers;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Identity.Controllers
{
    [Route("api")]
    [ApiController]
    public class RolesController : TweezersControllerBase
    {
        protected override bool WithInternalObjects => true;
        protected string CollectionName => IdentityManager.RolesSchemaName;

        [HttpGet("tweezers-roles")]
        public ActionResult<TweezersMultipleResults<JObject>> List([FromQuery] int skip = 0, [FromQuery] int take = 10,
            [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
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
            return WrapWithAuthorizationCheck(() =>
            {
                try
                {
                    TweezersObject objectMetadata =
                        TweezersSchemaFactory.Find(CollectionName, WithInternalObjects, true);
                    JObject role = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id, true);
                    if (role == null)
                    {
                        return TweezersOk(TweezersGeneralResponse.Create("Deleted"));
                    }

                    if (role["isBuiltInRole"]?.ToString().ToLower() == "true")
                    {
                        return TweezersBadRequest("Cannot delete a built-in role");
                    }

                    long count = IdentityManager.GetUsersByRoleId(id).Count;
                    if (count > 0)
                    {
                        return TweezersBadRequest(
                            $"Role is being used by {count} users, please change their role first.");
                    }

                    bool deleted = objectMetadata.Delete(TweezersSchemaFactory.DatabaseProxy, id);
                    return TweezersOk();
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Delete", DefaultPermission.Edit, CollectionName);
        }
    }
}