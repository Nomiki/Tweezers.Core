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
        /// <summary>
        /// Query a list of items from a collection
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="skip">How many items to skip</param>
        /// <param name="take">How many items to take</param>
        /// <param name="sortField">Sort the results by this field</param>
        /// <param name="direction">'asc' or 'desc'</param>
        /// <returns>A list of items from a collection, sorted and paginated</returns>
        [HttpGet("{collection}")]
        public virtual ActionResult<TweezersMultipleResults<JObject>> List(string collection,
            [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string sortField = "",
            [FromQuery] string direction = "asc")
        {
            return base.List(collection, skip, take, sortField, direction);
        }

        /// <summary>
        /// Query a specific item from a collection by an ID
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="id">Item's ID</param>
        /// <returns>The desired item from the collection</returns>
        [HttpGet("{collection}/{id}")]
        public virtual ActionResult<JObject> Get(string collection, string id)
        {
            return base.Get(collection, id);
        }

        /// <summary>
        /// Create a new item in the collection
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="data">Data to be inserted as the item</param>
        /// <param name="suggestedId">Optional ID for the item</param>
        /// <returns>The newly created item</returns>
        [HttpPost("{collection}")]
        public virtual ActionResult<JObject> Post(string collection, [FromBody] JObject data, string suggestedId = null)
        {
            return base.Post(collection, data, suggestedId);
        }

        /// <summary>
        /// Modifies an item in the collection
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="id">Item ID</param>
        /// <param name="data">Change set</param>
        /// <returns>The changed item</returns>
        [HttpPatch("{collection}/{id}")]
        public virtual ActionResult<JObject> Patch(string collection, string id, [FromBody] JObject data)
        {
            return base.Patch(collection, id, data);
        }

        /// <summary>
        /// Deletes an item from the collection
        /// </summary>
        /// <param name="collection">Collection name</param>
        /// <param name="id">Item ID</param>
        /// <returns>200 if the item was deleted</returns>
        [HttpDelete("{collection}/{id}")]
        public virtual ActionResult<JObject> Delete(string collection, string id)
        {
            return base.Delete(collection, id);
        }
    }
}