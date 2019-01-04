using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Engine;

namespace Tweezers.Api.Controllers
{
    [Route("api/tweezers")]
    [ApiController]
    public class TweezersEntryPoint : ControllerBase
    {
        private static List<DiscoverableMetadata> classList;

        [HttpGet]
        public ActionResult<List<DiscoverableMetadata>> Discover()
        {
            if (classList == null)
            {
                classList = Assembly.GetEntryAssembly().GetExportedTypes()
                    .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                    .Select(t => t.BaseType.GenericTypeArguments.Single())
                    .Select(DiscoveryEngine.GetData)
                    .ToList();
            }

            return Ok(classList);
        }
    }
}
