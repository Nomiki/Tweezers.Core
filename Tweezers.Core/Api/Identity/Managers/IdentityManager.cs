using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Identity.Managers
{
    public static class IdentityManager
    {
        public const string SessionIdKey = "sessionId";
        public const string SessionExpiryKey = "sessionExpiry";
        public const string UsersCollectionName = "tweezers-users";
        public const string RolesSchemaName = "tweezers-roles";

        public static bool UsingIdentity { get; private set; }

        public static void RegisterIdentity()
        {
            var usersSchema = CreateUsersSchema();

            SafeAddSchema(usersSchema);

            TweezersObject rolesSchema = CreateRolesSchema();

            SafeAddSchema(rolesSchema);

            UsingIdentity = true;
        }

        private static void SafeAddSchema(TweezersObject usersSchema)
        {
            if (TweezersSchemaFactory.Find(usersSchema.CollectionName, true, safe: true) == null)
                TweezersSchemaFactory.AddObject(usersSchema);
            else
            {
                TweezersSchemaFactory.DeleteObject(usersSchema.CollectionName);
                TweezersSchemaFactory.AddObject(usersSchema);
            }
        }

        private static TweezersObject CreateUsersSchema()
        {
            return Schemas.IdentityMetaJson.Deserialize<TweezersObject>();
        }

        private static TweezersObject CreateRolesSchema()
        {
            TweezersObject rolesInitialSchema = Schemas.RolesMetaJson.Deserialize<TweezersObject>();

            IEnumerable<TweezersObject> otherObjects = TweezersSchemaFactory.GetAll(true).Append(rolesInitialSchema);

            TweezersObject permissionsObjectReference = 
                rolesInitialSchema.Fields["permissions"].FieldProperties.ObjectReference;

            foreach (TweezersObject @object in otherObjects)
            {
                TweezersFieldProperties fieldProperties =
                    Schemas.PermissionTemplateFieldProperties.Deserialize<TweezersFieldProperties>();

                fieldProperties.Name = @object.CollectionName;
                fieldProperties.DisplayName = @object.PluralName;

                if (permissionsObjectReference.Fields.ContainsKey(@object.CollectionName))
                    continue;

                permissionsObjectReference.Fields.Add(@object.CollectionName, new TweezersField() {FieldProperties = fieldProperties});
            }

            return rolesInitialSchema;
        }

        public static JObject FindUserBySessionId(string sessionId, bool allFields = false)
        {
            FindOptions<JObject> sessionIdPredicate = new FindOptions<JObject>()
            {
                Predicate = (u) =>
                {
                    bool sessionIdEq = u[SessionIdKey].ToString().Equals(sessionId);
                    var sessionExpiryTime = long.Parse(u[SessionExpiryKey].ToString());
                    var now = DateTime.Now.ToFileTimeUtc();
                    bool sessionExpiryOk = sessionExpiryTime > now;
                    return sessionIdEq && sessionExpiryOk;
                },
                Take = 1,
            };

            TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find(UsersCollectionName, true, allFields);
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, sessionIdPredicate, allFields)?.Items.SingleOrDefault();
        }
    }
}
