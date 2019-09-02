using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Identity.Managers;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected TimeSpan SessionTimeout => 4.Hours();
        protected virtual bool WithInternalObjects => false;

        #region Pseudo Routes

        protected ActionResult<TweezersMultipleResults<JObject>> List(string collection,
            int skip = 0, int take = 10, string sortField = "", string direction = "asc")
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();

                Thread.Sleep(500);

                FindOptions<JObject> predicate = new FindOptions<JObject>()
                {
                    Skip = skip,
                    Take = take,
                    SortField = sortField,
                    SortDirection = direction == "asc" ? SortDirection.Ascending : SortDirection.Descending,
                };

                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                TweezersMultipleResults<JObject> results =
                    objectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, predicate);
                return TweezersOk(results);
            }
            catch (TweezersValidationException)
            {
                return TweezersNotFound();
            }
        }

        protected ActionResult<JObject> Get(string collection, string id)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();

                Thread.Sleep(500);

                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return TweezersNotFound();

                return TweezersOk(obj);
            }
            catch (TweezersValidationException)
            {
                return TweezersNotFound();
            }
        }

        protected ActionResult<JObject> Post(string collection, JObject data, string suggestedId = null)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                objectMetadata.Validate(data, false);
                JObject createdObj = objectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, data, suggestedId);
                return TweezersCreated(createdObj);
            }
            catch (TweezersValidationException e)
            {
                return TweezersForbidden("create", e.Message);
            }
        }

        protected ActionResult<JObject> Patch(string collection, string id, JObject data)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                JObject obj = objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id);
                if (obj == null)
                    return TweezersNotFound();

                objectMetadata.Validate(data, true);
                JObject updatedObj = objectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, id, data);
                return TweezersOk(updatedObj);
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }

        protected ActionResult<JObject> Delete(string collection, string id)
        {
            try
            {
                if (!IsSessionValid())
                    return TweezersUnauthorized();


                TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                if (objectMetadata.GetById(TweezersSchemaFactory.DatabaseProxy, id) == null)
                {
                    return TweezersOk(TweezersGeneralResponse.Create("Deleted"));
                }

                bool deleted = objectMetadata.Delete(TweezersSchemaFactory.DatabaseProxy, id);
                return TweezersOk();
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }

        #endregion

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
