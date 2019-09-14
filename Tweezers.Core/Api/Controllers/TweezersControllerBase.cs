using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            int skip = 0, int take = 10, string sortField = "", string direction = "asc",
            DefaultPermission? minimalPermission = null)
        {
            return WrapWithAuthorizationCheck(() =>
                {
                    try
                    {
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

                        HandleReferenceObjects(objectMetadata, results);

                        return TweezersOk(results);
                    }
                    catch (TweezersValidationException)
                    {
                        return TweezersNotFound();
                    }
                }, "List", minimalPermission ?? DefaultPermission.View, collection);
        }

        private static void HandleReferenceObjects(TweezersObject objectMetadata, TweezersMultipleResults<JObject> results)
        {
            IEnumerable<KeyValuePair<string, TweezersField>> fields =
                objectMetadata.Fields.Where(f =>
                    f.Value.FieldProperties.FieldType == TweezersFieldType.Object && 
                    !string.IsNullOrWhiteSpace(f.Value.FieldProperties.ObjectName));

            Dictionary<string, TweezersObject> referenceObjects = fields
                .Select(f =>
                {
                    string key = f.Key;
                    TweezersObject value = TweezersSchemaFactory.Find(f.Value.FieldProperties.ObjectName,
                        true, true, true);
                    return new KeyValuePair<string, TweezersObject>(key, value);
                })
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var obj in referenceObjects)
            {
                IEnumerable<string> objIds = results.Items.Select(r => r[obj.Key].ToString()).Distinct();
                string titleFieldId = obj.Value.Fields.Single(f => f.Value.FieldProperties.UiTitle).Key;
                Dictionary<string, string> idToTitle = objIds.Select(id =>
                    {
                        string title =
                            obj.Value.GetById(TweezersSchemaFactory.DatabaseProxy, id, true)[titleFieldId]
                                .ToString();
                        return new KeyValuePair<string, string>(id, title);
                    })
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (JObject item in results.Items)
                {
                    item[obj.Key] = idToTitle[item[obj.Key].ToString()];
                }
            }
        }

        protected ActionResult<JObject> Get(string collection, string id, DefaultPermission? minimalPermission = null)
        {
            return WrapWithAuthorizationCheck(() =>
                {
                    try
                    {
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
                }, "Get", minimalPermission ?? DefaultPermission.View, collection);
        }

        protected ActionResult<JObject> Post(string collection, JObject data, string suggestedId = null,
            DefaultPermission? minimalPermission = null)
        {
            return WrapWithAuthorizationCheck(() =>
                {
                    try
                    {
                        TweezersObject objectMetadata = TweezersSchemaFactory.Find(collection, WithInternalObjects);
                        objectMetadata.Validate(data, false);
                        JObject createdObj =
                            objectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, data, suggestedId);
                        return TweezersCreated(createdObj);
                    }
                    catch (TweezersValidationException e)
                    {
                        return TweezersBadRequest(e.Message);
                    }
                }, "Post", minimalPermission ?? DefaultPermission.Edit, collection);
        }

        protected ActionResult<JObject> Patch(string collection, string id, JObject data,
            DefaultPermission? minimalPermission = null)
        {
            return WrapWithAuthorizationCheck(() =>
                {
                    try
                    {
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
                }, "Patch", minimalPermission ?? DefaultPermission.Edit, collection);
        }

        protected ActionResult<JObject> Delete(string collection, string id,
            DefaultPermission? minimalPermission = null)
        {
            return WrapWithAuthorizationCheck(() =>
                {
                    try
                    {
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
                }, "Delete", minimalPermission ?? DefaultPermission.Edit, collection);
        }

        #endregion

        protected ActionResult TweezersForbidden(string method, string message)
        {
            return StatusCode(403, new TweezersErrorBody() {Message = message, Method = method});
        }

        protected ActionResult TweezersNotFound()
        {
            return StatusCode(404, new TweezersErrorBody() {Message = "Not found."});
        }

        protected ActionResult TweezersBadRequest(string message)
        {
            return StatusCode(400, new TweezersErrorBody() {Message = message});
        }

        protected ActionResult TweezersUnauthorized(string message = null)
        {
            return StatusCode(401, new TweezersErrorBody() {Message = message ?? "Unauthorized"});
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

        protected ActionResult WrapWithAuthorizationCheck(Func<ActionResult> internalLogic, string method,
            DefaultPermission? minimalRequestedPermission = null, string collection = null)
        {
            HttpStatusCode userSessionStatusCode = CheckUserSession(minimalRequestedPermission, collection);
            if (userSessionStatusCode == HttpStatusCode.Forbidden)
            {
                return TweezersForbidden(method, $"Access denied for {method}: {collection}");
            }

            if (userSessionStatusCode == HttpStatusCode.Unauthorized)
            {
                return TweezersUnauthorized();
            }

            return internalLogic.Invoke();
        }

        private HttpStatusCode CheckUserSession(DefaultPermission? minimalRequestedPermission = null,
            string collection = null)
        {
            HttpStatusCode sessionValidity = CheckSessionValidity(out var user);
            if (sessionValidity == HttpStatusCode.OK && user != null)
            {
                return CheckRoleValidity(minimalRequestedPermission, collection, user);
            }

            return sessionValidity;
        }

        private static HttpStatusCode CheckRoleValidity(DefaultPermission? minimalRequestedPermission,
            string collection,
            JObject user)
        {
            try
            {
                if (minimalRequestedPermission == null || minimalRequestedPermission == DefaultPermission.None)
                    return HttpStatusCode.OK;

                JObject role = IdentityManager.GetRoleById(user["roleId"].ToString());
                int userPermissionLevel = (int) Enum.Parse(typeof(DefaultPermission),
                    role["permissions"][collection].ToString(), true);
                bool roleValid = userPermissionLevel >= (int) minimalRequestedPermission;
                return roleValid ? HttpStatusCode.OK : HttpStatusCode.Forbidden;
            }
            catch (Exception)
            {
                return HttpStatusCode.Forbidden;
            }
        }

        private HttpStatusCode CheckSessionValidity(out JObject user)
        {
            user = null;
            if (!IdentityManager.UsingIdentity)
                return HttpStatusCode.OK;

            if (!Request.Headers.ContainsKey(IdentityManager.SessionIdKey))
                return HttpStatusCode.Unauthorized;

            try
            {
                user = GetUserBySessionId();
                return user == null ? HttpStatusCode.Unauthorized : HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return HttpStatusCode.Unauthorized;
            }
        }

        protected JObject GetUserBySessionId()
        {
            string sessionId = Request.Headers[IdentityManager.SessionIdKey];
            return IdentityManager.FindUserBySessionId(sessionId);
        }
    }
}