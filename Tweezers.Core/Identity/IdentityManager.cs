using Tweezers.Schema.DataHolders;

namespace Tweezers.Identity
{
    public static class IdentityManager
    {
        public static void RegisterIdentity()
        {
            TweezersObject usersSchema = new TweezersObject()
            {
                CollectionName = "users",
                Internal = true,
            };

            usersSchema.DisplayNames.SingularName = "user";
            usersSchema.DisplayNames.PluralName = "users";

            usersSchema.Fields.Add("username", new TweezersField()
            {
                Name = "username",
                DisplayName = "Username",
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                    UiTitle = true,
                    Min = 1,
                    Max = 50,
                    Required = true,
                    Regex = @"[A-Za-z\d]+"
                }
            });

            usersSchema.Fields.Add("passwordHash", new TweezersField()
            {
                Name = "passwordHash",
                DisplayName = "password",
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                    Required = true,
                    UiIgnore = true,
                }
            });

            usersSchema.Fields.Add("sessionId", new TweezersField()
            {
                Name = "sessionId",
                DisplayName = "Session ID", 
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String,
                }
            });

            usersSchema.Fields.Add("sessionExpiry", new TweezersField()
            {
                Name = "sessionExpiry",
                DisplayName = "SessionExpiry",
                FieldProperties = new TweezersFieldProperties()
                {
                    FieldType = TweezersFieldType.String
                }
            });

            TweezersSchemaFactory.AddObject(usersSchema);
        }
    }
}
