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
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();

                Thread.Sleep(500);

                FindOptions<JObject> predicate = new FindOptions<JObject>()
                {
                    Skip = skip,
                    Take = take,
                    SortField = sortField,
                    SortDirection = direction == "asc" ? SortDirection.Ascending : SortDirection.Descending,
                };

                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                TweezersMultipleResults<JObject> results = 
                    objectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, predicate);
                return TweezersOk(results);
            }
            catch (TweezersValidationException)
            {
                return TweezersNotFound();
            }
        }

        [HttpGet("{collection}/{id}")]
        public virtual ActionResult<JObject> Get(string collection, string id)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return TweezersNotFound();

                return TweezersOk(obj);
            }
            catch (TweezersValidationException)
            {
                return TweezersNotFound();
            }
        }

        [HttpPost("{collection}")]
        public virtual ActionResult<JObject> Post(string collection, [FromBody] JObject data)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                objectMetadata.Validate(data, false);
                JObject createdObj = objectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, data);
                return TweezersCreated(createdObj);
            }
            catch (TweezersValidationException e)
            {
                return TweezersForbidden("create", e.Message);
            }
        }

        [HttpPatch("{collection}/{id}")]
        public virtual ActionResult<JObject> Patch(string collection, string id, [FromBody] JObject data)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return TweezersNotFound();

                objectMetadata.Validate(data, true);
                JObject updatedObj = objectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, id, data);
                return TweezersOk(updatedObj);
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }

        [HttpDelete("{collection}/{id}")]
        public virtual ActionResult<bool> Delete(string collection, string id)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                if (objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id) == null)
                {
                    return TweezersOk(true);
                }

                bool deleted = objectMetadata.Delete(TweezersSchemaFactory.DatabaseProxy, id);
                return TweezersOk();
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }
    }
}