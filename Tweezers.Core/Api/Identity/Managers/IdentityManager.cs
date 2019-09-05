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

            CreateDefaultRoles();

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

            IEnumerable<TweezersObject> additionalInternalSchemas = new[] {rolesInitialSchema};

            if (SchemaManagement.CanChangeSchema)
            {
                additionalInternalSchemas = additionalInternalSchemas.Append(SchemaManagement.SchemaMetadata);
            }

            IEnumerable<TweezersObject> otherObjects =
                TweezersSchemaFactory.GetAll(true).Concat(additionalInternalSchemas);

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

                permissionsObjectReference.Fields.Add(@object.CollectionName,
                    new TweezersField() {FieldProperties = fieldProperties});
            }

            return rolesInitialSchema;
        }

        public static void AppendNewPermission(TweezersObject newObject)
        {
            TweezersObject rolesObj = TweezersSchemaFactory.Find(RolesSchemaName, true, true);

            TweezersFieldProperties fieldProperties =
                Schemas.PermissionTemplateFieldProperties.Deserialize<TweezersFieldProperties>();

            fieldProperties.Name = newObject.CollectionName;
            fieldProperties.DisplayName = newObject.PluralName;

            rolesObj.Fields["permissions"].FieldProperties.ObjectReference.Fields.Add(newObject.CollectionName,
                new TweezersField() {FieldProperties = fieldProperties});

            SafeAddSchema(rolesObj);

            DefaultPermission permission = newObject.DefaultPermission;
            TweezersMultipleResults<JObject> roles = rolesObj.FindInDb(TweezersSchemaFactory.DatabaseProxy, FindOptions<JObject>.Default(0, int.MaxValue), true);
            foreach (JObject role in roles.Items)
            {
                role["permissions"][newObject.CollectionName] = permission.ToString();
                rolesObj.Update(TweezersSchemaFactory.DatabaseProxy, role["_id"].ToString(), role);
            }
        }

        public static void EditPermissionName(TweezersObject editedObject)
        {
            TweezersObject rolesObj = TweezersSchemaFactory.Find(RolesSchemaName, true, true);

            TweezersField field = rolesObj.Fields["permissions"].FieldProperties.ObjectReference.Fields[editedObject.CollectionName];
            field.FieldProperties.DisplayName = editedObject.PluralName;

            SafeAddSchema(rolesObj);
        }

        public static void DeletePermission(string deletedCollectionName)
        {
            TweezersObject rolesObj = TweezersSchemaFactory.Find(RolesSchemaName, true, true);

            rolesObj.Fields["permissions"].FieldProperties.ObjectReference.Fields.Remove(deletedCollectionName);

            SafeAddSchema(rolesObj);

            TweezersMultipleResults<JObject> roles = rolesObj.FindInDb(TweezersSchemaFactory.DatabaseProxy, FindOptions<JObject>.Default(0, int.MaxValue), true);
            foreach (JObject role in roles.Items)
            {
                JObject permissions = JObject.FromObject(role["permissions"]);
                permissions.Remove(deletedCollectionName);
                role["permissions"] = permissions;
                rolesObj.Update(TweezersSchemaFactory.DatabaseProxy, role["_id"].ToString(), role);
            }
        }

        private static void CreateDefaultRoles()
        {
            JObject administrator = Schemas.AdministratorRoleTemplate.Deserialize<JObject>();
            TweezersObject rolesObj = TweezersSchemaFactory.Find(RolesSchemaName, true, true);

            if (rolesObj.GetById(TweezersSchemaFactory.DatabaseProxy, administrator["_id"].ToString()) == null)
            {
                JObject permissions = new JObject();
                foreach (string collectionName in rolesObj.Fields["permissions"].FieldProperties.ObjectReference.Fields.Keys)
                {
                    permissions.Add(collectionName, DefaultPermission.Edit.ToString());
                }

                administrator["permissions"] = permissions;

                rolesObj.Create(TweezersSchemaFactory.DatabaseProxy, administrator, administrator["_id"].ToString());
            }
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
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, sessionIdPredicate, allFields)
                ?.Items.SingleOrDefault();
        }
    }
}