using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Identity;
using Tweezers.Api.Utils;
using Tweezers.Schema.Common;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected TimeSpan SessionTimeout => 4.Hours();

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

        protected ActionResult TweezersUnauthorized(string message = null)
        {
            return StatusCode(401, new TweezersErrorBody() { Message = message ?? "Unauthorized" });
        }

        protected ActionResult TweezersOk(object obj = null)
        {
            return StatusCode(200, ResolveByContract(obj ?? TweezersGeneralResponse.Create("Ok")));
        }

        protected ActionResult TweezersOk(object obj, params string[] removeFields)
        {
            return StatusCode(200, ResolveByContract(obj).Without(removeFields));
        }

        protected ActionResult TweezersCreated(object obj)
        {
            return StatusCode(201, ResolveByContract(obj));
        }

        private JObject ResolveByContract(object obj)
        {
            return JObject.FromObject(obj, Serializer.JsonSerializer);
        }

        protected bool IsSessionValid()
        {
            if (!IdentityManager.UsingIdentity)
                return true;

            if (!Request.Headers.ContainsKey(IdentityManager.SessionIdKey))
                return false;

            try
            {
                JObject user = GetUserBySessionId();
                return user != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected JObject GetUserBySessionId()
        {
            string sessionId = Request.Headers[IdentityManager.SessionIdKey];
            return IdentityManager.FindUserBySessionId(sessionId);
        }
    }
}
