using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Database;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Exceptions;
using Tweezers.Api.Interfaces;
using Tweezers.Discoveries.Attributes;
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

        [HttpGet("tweezers/[controller]")]
        public ActionResult<DiscoverableMetadata> TweezIt()
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return DiscoveryError();

                return Ok(TweezersMetadata);
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
            }
        }

        // GET api/[controller]
        [HttpGet("[controller]")]
        public virtual ActionResult<IEnumerable<T>> List()
        {
            try
            {
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return ForbiddenResult("get", $"Could not list items of type {typeof(T).Name}");

                return Ok(DatabaseProxy.List<T>(FindOptions<T>.Default()).Select(DeleteTweezersIgnores));
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
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
                string error = $"Could not get {typeof(T).Name} by ID:{id}";
                if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                    return ForbiddenResult("get", error);

                T obj = DatabaseProxy.Get<T>(id);

                return Ok(DeleteTweezersIgnores(obj));
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
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
                    return ForbiddenResult("post", $"Could not add {typeof(T).Name}");

                string id = Guid.NewGuid().ToString();
                PropertyInfo idProperty = DetermineIdAttr<T>();
                idProperty.SetValue(value, id);
                T newObj = DatabaseProxy.Add<T>(id, value);
                return Ok(DeleteTweezersIgnores(newObj));
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
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
                    return ForbiddenResult("patch", $"Could not edit {typeof(T).Name} by ID:{id}");

                T editedItem = DatabaseProxy.Edit<T>(id, value);
                return Ok(DeleteTweezersIgnores(editedItem));
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
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
                    return ForbiddenResult("delete", $"Could not delete {id}");

                DatabaseProxy.Delete<T>(id);
                return Ok();
            }
            catch (TweezersDiscoveryException)
            {
                return DiscoveryError();
            }
            catch (ItemNotFoundException e)
            {
                return NotFoundResult(e.Message);
            }
        }

        private ActionResult DiscoveryError()
        {
            return ForbiddenResult("tweezers", $"Could not discover {typeof(T).Name}");
        }
    }
}