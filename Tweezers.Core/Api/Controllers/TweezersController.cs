using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Database;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Exceptions;
using Tweezers.Api.Interfaces;
using Tweezers.Discoveries.Common;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Engine;
using Tweezers.Discoveries.Exceptions;
using Tweezers.Discoveries.Rbac;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public abstract class TweezersController<T> : TweezersControllerBase
    {
        protected DiscoverableMetadata TweezersMetadata => GetMetadata();

        protected TweezersController(IDatabaseProxy databaseProxy = null) : this(null, databaseProxy)
        {
        }

        protected TweezersController(string idFieldName, IDatabaseProxy databaseProxy = null)
        {
            DatabaseProxy = databaseProxy ?? LocalDatabase.Instance;
        }

        private DiscoverableMetadata GetMetadata()
        {
            return DiscoveryEngine.GetData(typeof(T));
        }

        protected ActionResult ForbiddenResult(string method, string id = null)
        {
            return new ForbidResult();
        }

        protected ActionResult NotFoundResult(string message)
        {
            return NotFound(new TweezersErrorBody()
            {
                Code = 404,
                Message = message
            });
        }

        [HttpGet("tweezers/[controller]")]
        public ActionResult<DiscoverableMetadata> TweezIt()
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return ForbiddenResult("tweezers", typeof(T).Name);

                return Ok(TweezersMetadata);
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
        }

        // GET api/[controller]
        [HttpGet("[controller]")]
        public virtual ActionResult<IEnumerable<T>> List()
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return ForbiddenResult("get", "list");

                return Ok(DatabaseProxy.List<T>(FindOptions<T>.Default()));
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        // GET api/[controller]/5
        [HttpGet("[controller]/{id}")]
        public virtual ActionResult<T> Get(string id)
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return ForbiddenResult("get", id);

                return Ok(DatabaseProxy.Get<T>(id));
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        // POST api/[controller]
        [HttpPost("[controller]")]
        public virtual ActionResult<T> Post([FromBody] T value)
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.View,
                    TweezersAllowedActions.None))
                    return ForbiddenResult("post");

                string id = Guid.NewGuid().ToString();
                PropertyInfo idProperty = DetermineIdAttr<T>();
                idProperty.SetValue(value, id);
                return Ok(DatabaseProxy.Add<T>(id, value));
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        [HttpPatch("[controller]/{id}")]
        public virtual ActionResult<T> Patch(string id, [FromBody] T value)
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.View,
                    TweezersAllowedActions.None))
                    return ForbiddenResult("patch", id);

                return Ok(DatabaseProxy.Edit<T>(id, value));
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        // DELETE api/[controller]/5
        [HttpDelete("[controller]/{id}")]
        public virtual ActionResult<T> Delete(string id)
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.Edit,
                    TweezersAllowedActions.View,
                    TweezersAllowedActions.None))
                    return ForbiddenResult("delete", id);

                DatabaseProxy.Delete<T>(id);
                return Ok();
            }
            catch (TweezersDiscoveryException)
            {
                return ForbiddenResult("tweezers", typeof(T).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }
    }
}