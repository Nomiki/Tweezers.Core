using System.Collections.Generic;
using Discoveries.Containers;
using Discoveries.Engine;
using Microsoft.AspNetCore.Mvc;

namespace DiscoveryApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweezersController<T> : ControllerBase
    {

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
            return null;
        }

        // GET api/[controller]/5
        [HttpGet("{id}")]
        public ActionResult<T> Get(string id)
        {
            return null;
        }

        // POST api/[controller]
        [HttpPost]
        public void Post([FromBody] T value)
        {
        }

        [HttpPatch("{id}")]
        public void Patch(string id, [FromBody] T obj)
        {
        }

        // DELETE api/[controller]/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
        }
    }
}