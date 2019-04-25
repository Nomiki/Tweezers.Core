using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TweezersController : TweezersControllerBase
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
             NullValueHandling = NullValueHandling.Ignore,
             Formatting = Formatting.Indented
        };

        private static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

        [HttpGet("tweezers")]
        public ActionResult<IEnumerable<JObject>> GetAllMetadata([FromQuery] bool full = false)
        {
            try
            {
                return new ActionResult<IEnumerable<JObject>>(TweezersSchemaFactory.GetAll(full).Select(obj => JObject.FromObject(obj, Serializer)));
            }
            catch (TweezersValidationException e)
            {
                return BadRequestResult(e.Message);
            }
        }

        [HttpGet("tweezers/{collection}")]
        public ActionResult<JObject> GetMetadata(string collection)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                return new ActionResult<JObject>(JObject.FromObject(objectMetadata, Serializer));
            }
            catch (TweezersValidationException)
            {
                return NotFoundResult();
            }
        }

        [HttpGet("{collection}")]
        public virtual ActionResult<IEnumerable<JObject>> List(string collection)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                return 
                    new ActionResult<IEnumerable<JObject>>(
                        objectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, FindOptions<JObject>.Default())
                            .Select(obj => JObject.FromObject(obj, Serializer))); // TODO: predicate
            }
            catch (TweezersValidationException)
            {
                return NotFoundResult();
            }
        }

        [HttpGet("{collection}/{id}")]
        public virtual ActionResult<JObject> Get(string collection, string id)
        {
            try
            {
                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection);
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return NotFoundResult();

                return new ActionResult<JObject>(obj);
            }
            catch (TweezersValidationException)
            {
                return NotFoundResult();
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
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return NotFoundResult();

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