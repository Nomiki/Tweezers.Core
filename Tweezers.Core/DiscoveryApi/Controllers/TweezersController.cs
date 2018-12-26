using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discoveries.Containers;
using Discoveries.Engine;
using DiscoveryApi.Attributes;
using DiscoveryApi.Database;
using DiscoveryApi.DataHolders;
using DiscoveryApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiscoveryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class TweezersController<T> : ControllerBase
    {
        internal PropertyInfo IdProperty { get; private set; }
        internal IDatabaseProxy DatabaseProxy { get; private set; }

        public TweezersController(IDatabaseProxy databaseProxy = null)
        {
            IdProperty = DetermineIdAttr();
            DatabaseProxy = databaseProxy ?? LocalDatabase.Instance;
        }

        public TweezersController(string idFieldName, IDatabaseProxy databaseProxy = null)
        {
            IdProperty = DetermineIdAttr(idFieldName);
            DatabaseProxy = databaseProxy ?? LocalDatabase.Instance;
        }

        private PropertyInfo DetermineIdAttr(string idFieldName = null)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return idFieldName == null
                ? properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any())
                : properties.Single(pi => pi.Name == idFieldName);
        }

        [HttpGet("tweez-it")]
        public ActionResult<DiscoverableMetadata> TweezIt()
        {
            DiscoverableMetadata metadata = DiscoveryEngine.GetData(typeof(T));
            return metadata != null ? (ActionResult<DiscoverableMetadata>) Ok(metadata) : NotFound();
        }

        // GET api/[controller]
        [HttpGet]
        public ActionResult<IEnumerable<T>> List()
        {
            return Ok(DatabaseProxy.List<T>(FindOptions<T>.Default()));
        }

        // GET api/[controller]/5
        [HttpGet("{id}")]
        public ActionResult<T> Get(string id)
        {
            return Ok(DatabaseProxy.Get<T>(id));
        }

        // POST api/[controller]
        [HttpPost]
        public ActionResult<T> Post([FromBody] T value)
        {
            string id = Guid.NewGuid().ToString();
            IdProperty.SetValue(value, id);
            return Ok(DatabaseProxy.Add<T>(id, value));
        }

        [HttpPatch("{id}")]
        public ActionResult<T> Patch(string id, [FromBody] T value)
        {
            return Ok(DatabaseProxy.Edit<T>(id, value));
        }

        // DELETE api/[controller]/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            DatabaseProxy.Delete<T>(id);
        }
    }
}