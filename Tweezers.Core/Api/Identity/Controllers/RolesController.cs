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
    public sealed class RolesController : TweezersControllerBase
    {
        protected override bool WithInternalObjects => true;
        private string CollectionName => IdentityManager.RolesSchemaName;

        /// <summary>
        /// Query all user roles
        /// </summary>
        /// <param name="skip">How many user roles to skip</param>
        /// <param name="take">How many user roles to take</param>
        /// <param name="sortField">Sort the results by this field</param>
        /// <param name="direction">'asc' or 'desc'</param>
        /// <returns>A list of user roles, sorted and paginated</returns>
        [HttpGet("tweezers-roles")]
        public ActionResult<TweezersMultipleResults<JObject>> List([FromQuery] int skip = 0, [FromQuery] int take = 10,
            [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            return base.List(CollectionName, skip, take, sortField, direction);
        }

        /// <summary>
        /// Query a specific user role from the DB
        /// </summary>
        /// <param name="id">Desired user role to query</param>
        /// <returns>The specifically queried user role</returns>
        [HttpGet("tweezers-roles/{id}")]
        public ActionResult<JObject> Get(string id)
        {
            return base.Get(CollectionName, id);
        }

        /// <summary>
        /// Create a new user role
        /// </summary>
        /// <param name="data">Created user role data</param>
        /// <returns>The newly created user role</returns>
        [HttpPost("tweezers-roles")]
        public ActionResult<JObject> Post([FromBody] JObject data)
        {
            return base.Post(CollectionName, data, data["name"]?.ToString());
        }

        /// <summary>
        /// Modifies a user role
        /// </summary>
        /// <param name="id">User role id to be modified</param>
        /// <param name="data">Change set</param>
        /// <returns>The modified user role</returns>
        [HttpPatch("tweezers-roles/{id}")]
        public ActionResult<JObject> Patch(string id, [FromBody] JObject data)
        {
            return base.Patch(CollectionName, id, data);
        }

        /// <summary>
        /// Deletes a user role
        /// </summary>
        /// <param name="id">User role id to be deleted</param>
        /// <returns>200 if the user role was deleted</returns>
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