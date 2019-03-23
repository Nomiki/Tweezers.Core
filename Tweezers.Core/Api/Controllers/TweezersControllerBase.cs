using System.Buffers;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Interfaces;
using Tweezers.Discoveries.Attributes;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected IDatabaseProxy DatabaseProxy { get; set; }

        public PropertyInfo DetermineIdAttr<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any());
        }

        protected T DeleteTweezersIgnores<T>(T obj)
        {
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                bool isIgnored = prop.GetCustomAttributes<TweezersIgnoreAttribute>().Any();
                if (isIgnored)
                    prop.SetValue(obj, null);
            }

            return obj;
        }

        protected ActionResult ForbiddenResult(string method, string id = null)
        {
            return StatusCode(403, new TweezersErrorBody() { Message = $"{method}: forbidden {id}" });
        }

        protected ActionResult NotFoundResult(string message)
        {
            return StatusCode(404, new TweezersErrorBody() {Message = message});
        }
    }
}
