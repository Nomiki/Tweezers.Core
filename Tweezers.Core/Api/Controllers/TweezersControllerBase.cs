using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweezers.Api.DataHolders;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        private static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

        protected ActionResult TweezersForbidden(string method, string message)
        {
            return StatusCode(403, new TweezersErrorBody() { Message = message, Method = method });
        }

        protected ActionResult TweezersNotFound()
        {
            return StatusCode(404, new TweezersErrorBody() { Message = "Not found." });
        }

        protected ActionResult TweezersBadRequest(string message)
        {
            return StatusCode(400, new TweezersErrorBody() { Message = message });
        }

        protected ActionResult TweezersUnauthorized(string message)
        {
            return StatusCode(401, new TweezersErrorBody() { Message = message });
        }

        protected ActionResult TweezersOk(object obj)
        {
            return StatusCode(200, RemoveNulls(obj));
        }

        protected ActionResult TweezersCreated(object obj)
        {
            return StatusCode(201, RemoveNulls(obj));
        }

        private JObject RemoveNulls(object obj)
        {
            return JObject.FromObject(obj, Serializer);
        }
    }
}
