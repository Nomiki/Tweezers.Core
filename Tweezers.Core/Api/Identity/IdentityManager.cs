using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.Schema.DataHolders;

namespace Tweezers.Api.Identity
{
    public static class IdentityManager
    {
        public const string SessionIdKey = "sessionId";
        public const string SessionExpiryKey = "sessionExpiry";
        public const string UsersCollectionName = "tweezers-users";

        public static bool UsingIdentity { get; private set; }

        public static void RegisterIdentity()
        {
            var usersSchema = CreateUsersSchema(true);

            if (TweezersSchemaFactory.Find(usersSchema.CollectionName, true, safe: true) == null)
                TweezersSchemaFactory.AddObject(usersSchema);
            else
            {
                TweezersSchemaFactory.DeleteObject(usersSchema.CollectionName);
                TweezersSchemaFactory.AddObject(usersSchema);
            }

            UsingIdentity = true;
        }

        public static TweezersObject CreateUsersSchema(bool withInternals = false)
        {
            return Schemas.IdentityMetaJson.Deserialize<TweezersObject>();
        }

        public static JObject FindUserBySessionId(string sessionId)
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

            TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find(UsersCollectionName, true);
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, sessionIdPredicate)?.Items.SingleOrDefault();
        }
    }
}
