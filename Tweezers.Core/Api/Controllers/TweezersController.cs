using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public abstract class TweezersController : TweezersControllerBase
    {
        [HttpGet("tweezers/{collection}")]
        public ActionResult<JObject> GetMetadata(string collection)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                return new ActionResult<JObject>(JObject.FromObject(objectMetadata));
            }
            catch (TweezersValidationException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        [HttpGet("{collection}")]
        public virtual ActionResult<IEnumerable<JObject>> List(string collection)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                return new ActionResult<IEnumerable<JObject>>(
                    objectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, FindOptions<JObject>.Default())); // TODO: predicate
            }
            catch (TweezersValidationException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        [HttpGet("{collection}/{id}")]
        public virtual ActionResult<JObject> Get(string collection, string id)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                return new ActionResult<JObject>(objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id));
            }
            catch (TweezersValidationException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        [HttpPost("{collection}")]
        public virtual ActionResult<JObject> Post(string collection, [FromBody] JObject data)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                objectMetadata.Validate(data, false);
                return new ActionResult<JObject>(objectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, data));
            }
            catch (TweezersValidationException e)
            {
                return ForbiddenResult("create", e.Message);
            }
        }

        [HttpPatch("{collection}/{id}")]
        public virtual ActionResult<JObject> Patch(string collection, string id, [FromBody] JObject data)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                objectMetadata.Validate(data, true);
                return new ActionResult<JObject>(objectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, id, data));
            }
            catch (TweezersValidationException e)
            {
                return BadRequestResult(e.Message);
            }
        }

        [HttpDelete("{collection}/{id}")]
        public virtual ActionResult<bool> Delete(string collection, string id)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                if (objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id) == null)
                {
                    return new ActionResult<bool>(true);
                }

                return new ActionResult<bool>(objectMetadata.Delete(TweezersSchemaFactory.DatabaseProxy, id));
            }
            catch (TweezersValidationException e)
            {
                return BadRequestResult(e.Message);
            }
        }
    }
}