using System;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;
using Tweezers.Identity.HashUtils;

namespace Tweezers.Identity.DataHolders
{
    [TweezersEntity("User", AllowedActions = TweezersAllowedActions.Admin, IconName = "perm_identity")]
    public sealed class User
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Username")]
        public string Username { get; set; }

        [TweezersIgnore]
        public string PasswordHash { get; set; }

        [TweezersIgnore]
        public string SessionId { get; set; }

        [TweezersIgnore]
        public DateTime? SessionExpiry { get; set; }

        // TODO: Role

        public static User CreateUser(LoginRequest suggestedUser) 
        {
            return new User()
            {
                Id = Guid.NewGuid().ToString(),
                Username = suggestedUser.Username,
                PasswordHash = Hash.Create(suggestedUser.Password)
            };
        }

        public bool ValidatePassword(string requestPassword)
        {
            return Hash.Validate(requestPassword, PasswordHash);
        }
    }
}
