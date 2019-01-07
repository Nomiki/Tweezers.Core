using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Database;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Interfaces;
using Tweezers.Discoveries.Common;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Engine;
using Tweezers.Discoveries.Rbac;

namespace Tweezers.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public abstract class TweezersController<T> : TweezersControllerBase
    {
        protected IDatabaseProxy DatabaseProxy { get; private set; }
        protected DiscoverableMetadata TweezersMetadata { get; private set; }

        protected TweezersController(IDatabaseProxy databaseProxy = null) : this(null, databaseProxy)
        {
        }

        protected TweezersController(string idFieldName, IDatabaseProxy databaseProxy = null)
        {
            DatabaseProxy = databaseProxy ?? LocalDatabase.Instance;
            TweezersMetadata = GetMetadata();
        }

        private DiscoverableMetadata GetMetadata()
        {
            // TODO: log it if fails
            return DiscoveryEngine.GetData(typeof(T));
        }

        protected ActionResult ForbiddenResult(string method, string id = null)
        {
            return NotFound();
        }

        [HttpGet("tweezers/[controller]")]
        public ActionResult<DiscoverableMetadata> TweezIt()
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                return ForbiddenResult("tweezers");

            return Ok(TweezersMetadata);
        }

        // GET api/[controller]
        [HttpGet("[controller]")]
        public virtual ActionResult<IEnumerable<T>> List()
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                return ForbiddenResult("get", "list");

            return Ok(DatabaseProxy.List<T>(FindOptions<T>.Default()));
        }

        // GET api/[controller]/5
        [HttpGet("[controller]/{id}")]
        public virtual ActionResult<T> Get(string id)
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.None))
                return ForbiddenResult("get", id);

            return Ok(DatabaseProxy.Get<T>(id));
        }

        // POST api/[controller]
        [HttpPost("[controller]")]
        public virtual ActionResult<T> Post([FromBody] T value)
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.View, TweezersAllowedActions.None))
                return ForbiddenResult("post");

            string id = Guid.NewGuid().ToString();
            PropertyInfo idProperty = GetIdProperty<T>();
            idProperty.SetValue(value, id);
            return Ok(DatabaseProxy.Add<T>(id, value));
        }

        [HttpPatch("[controller]/{id}")]
        public virtual ActionResult<T> Patch(string id, [FromBody] T value)
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.View, TweezersAllowedActions.None))
                return ForbiddenResult("patch", id);

            return Ok(DatabaseProxy.Edit<T>(id, value));
        }

        // DELETE api/[controller]/5
        [HttpDelete("[controller]/{id}")]
        public virtual ActionResult<T> Delete(string id)
        {
            if (TweezersMetadata.EntityData.AllowedActions.In(TweezersAllowedActions.Edit, TweezersAllowedActions.View,
                TweezersAllowedActions.None))
                return ForbiddenResult("delete", id);

            DatabaseProxy.Delete<T>(id);
            return Ok();
        }
    }
}