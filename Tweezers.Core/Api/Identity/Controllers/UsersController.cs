using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Common;
using Tweezers.Api.Controllers;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Identity.DataHolders;
using Tweezers.Api.Identity.HashUtils;
using Tweezers.Api.Identity.Managers;
using Tweezers.Api.Schema;
using Tweezers.Api.Utils;
using Tweezers.DBConnector;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.Exceptions;

namespace Tweezers.Api.Identity.Controllers
{
    [Route("api")]
    [ApiController]
    public sealed class UsersController : TweezersControllerBase
    {
        private static readonly TweezersObject UsersLoginSchema;

        protected override bool WithInternalObjects => true;

        static UsersController()
        {
            UsersLoginSchema = Schemas.UserExternalSchema.Deserialize<TweezersObject>();
        }

        private readonly string[] _loginResponseBody =
            {"username", IdentityManager.SessionIdKey, IdentityManager.SessionExpiryKey, "name"};

        /// <summary>
        /// Query all users metadata from the DB
        /// </summary>
        /// <returns>A list of users metadata</returns>
        [HttpGet("tweezers-schema/tweezers-users")]
        public ActionResult<TweezersObject> GetUsersSchema()
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return WrapWithAuthorizationCheck(() => TweezersOk(UsersLoginSchema), "Get", DefaultPermission.View,
                IdentityManager.UsersCollectionName);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="suggestedUser">Requested user name for the newly created user</param>
        /// <returns>200 if the user was created successfully</returns>
        [HttpPost("tweezers-users")]
        public ActionResult<JObject> Post([FromBody] CreateUserRequest suggestedUser)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return WrapWithAuthorizationCheck(() =>
            {
                try
                {
                    if (FindUser(suggestedUser.Username) != null)
                    {
                        throw new TweezersValidationException(
                            TweezersValidationResult.Reject($"Unable to create user"));
                    }

                    TweezersValidationResult passwordOk =
                        UsersLoginSchema.Fields["password"].Validate(suggestedUser.Password);
                    if (!passwordOk.Valid)
                    {
                        throw new TweezersValidationException(passwordOk);
                    }

                    JObject user = CreateUser(suggestedUser);
                    return TweezersOk(user);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Post", DefaultPermission.Edit, IdentityManager.UsersCollectionName);
        }

        /// <summary>
        /// Modifies user
        /// </summary>
        /// <param name="id">User id to be modified</param>
        /// <param name="patchRequest">Change set</param>
        /// <returns>200 if the user was changed successfully</returns>
        [HttpPatch("tweezers-users/{id}")]
        public ActionResult<JObject> Patch(string id, [FromBody] CreateUserRequest patchRequest)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return WrapWithAuthorizationCheck(() =>
            {
                try
                {
                    if (FindUser(patchRequest.Username) == null)
                    {
                        throw new TweezersValidationException(TweezersValidationResult.Reject($"Unable to find user"));
                    }

                    JObject userJObject = JObject.FromObject(patchRequest, Serializer.JsonSerializer);
                    if (patchRequest.Password != null)
                    {
                        TweezersValidationResult passwordOk =
                            UsersLoginSchema.Fields["password"].Validate(patchRequest.Password);
                        if (!passwordOk.Valid)
                        {
                            throw new TweezersValidationException(passwordOk);
                        }

                        userJObject["passwordHash"] = Hash.Create(patchRequest.Password);
                    }

                    TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find
                        (IdentityManager.UsersCollectionName, true, true);
                    usersObjectMetadata.Validate(userJObject, true);
                    JObject user = usersObjectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, id, userJObject);
                    return TweezersOk(user);
                }
                catch (TweezersValidationException e)
                {
                    return TweezersBadRequest(e.Message);
                }
            }, "Patch", DefaultPermission.Edit, IdentityManager.UsersCollectionName);
        }

        public static JObject CreateUser(CreateUserRequest suggestedUser)
        {
            JObject user = new JObject
            {
                ["username"] = suggestedUser.Username,
                ["passwordHash"] = Hash.Create(suggestedUser.Password),
                ["name"] = suggestedUser.Name,
                ["roleId"] = suggestedUser.RoleId,
            };

            TweezersObject usersObjectMetadata =
                TweezersSchemaFactory.Find(IdentityManager.UsersCollectionName, true, true);
            usersObjectMetadata.Validate(user, false);
            user = usersObjectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, user, suggestedUser.Username);
            return user;
        }

        /// <summary>
        /// Query a list of users from the DB
        /// </summary>
        /// <param name="skip">How many users to skip</param>
        /// <param name="take">How many users to take</param>
        /// <param name="sortField">Sort the results by this field</param>
        /// <param name="direction">'asc' or 'desc'</param>
        /// <returns>A list of users, sorted and paginated</returns>
        [HttpGet("tweezers-users")]
        public ActionResult<TweezersMultipleResults<JObject>> List([FromQuery] int skip = 0, [FromQuery] int take = 10,
            [FromQuery] string sortField = "", [FromQuery] string direction = "asc")
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return base.List(IdentityManager.UsersCollectionName, skip, take, sortField, direction);
        }

        /// <summary>
        /// Query a specific user from the DB
        /// </summary>
        /// <param name="id">User id to be queried by</param>
        /// <returns>200 is the user was found in the DB</returns>
        [HttpGet("tweezers-users/{id}")]
        public ActionResult<JObject> Get(string id)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return base.Get(IdentityManager.UsersCollectionName, id);
        }

        /// <summary>
        /// Login request for a specific user
        /// </summary>
        /// <param name="request">The request contains a user name and password</param>
        /// <returns>Response with session key and session expiration of 4 hours</returns>
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            try
            {
                if (Authenticate(request, out JObject user))
                {
                    string sessionId = Guid.NewGuid().ToString();
                    user[IdentityManager.SessionIdKey] = sessionId;
                    user[IdentityManager.SessionExpiryKey] = (DateTime.Now + SessionTimeout).ToFileTimeUtc()
                        .ToString(CultureInfo.InvariantCulture);

                    TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find(IdentityManager.UsersCollectionName,
                        true, true);

                    usersObjectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, user["_id"].ToString(),
                        user.Just(IdentityManager.SessionIdKey, IdentityManager.SessionExpiryKey));

                    return TweezersOk(user.Just(_loginResponseBody))
                        .WithSecureCookie(Response, IdentityManager.SessionIdKey, sessionId);
                }

                return TweezersUnauthorized("Bad username or password");
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }

        /// <summary>
        /// Logout request for the asking user
        /// </summary>
        /// <returns>200 if the user was logged out. The session key is disabled.</returns>
        [HttpPost("logout")]
        public ActionResult Logout()
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            try
            {
                JObject user = GetUserBySessionId();
                if (user == null)
                {
                    return TweezersOk();
                }

                JObject payload = new JObject()
                {
                    [IdentityManager.SessionExpiryKey] = (DateTime.Now - 1.Hours()).ToFileTimeUtc().ToString()
                };

                TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find(IdentityManager.UsersCollectionName,
                    true, true);

                usersObjectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, user["_id"].ToString(), payload);

                return TweezersOk();
            }
            catch (TweezersValidationException e)
            {
                return TweezersBadRequest(e.Message);
            }
        }

        /// <summary>
        /// Deletes a user from the DB
        /// </summary>
        /// <param name="username">The desired username to be deleted</param>
        /// <returns>200 if the user was successfully deleted</returns>
        [HttpDelete("tweezers-users/{username}")]
        public ActionResult<JObject> DeleteUser(string username)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            if (GetUserBySessionId()["_id"].ToString() == username)
                return TweezersBadRequest("Cannot delete self");

            return base.Delete(IdentityManager.UsersCollectionName, username);
        }

        /// <summary>
        /// Request a password reset for a specific user
        /// </summary>
        /// <param name="changePasswordRequest">Password reset request, contains the username and the newly requested password</param>
        /// <returns>200 if the request is valid and the password was changed</returns>
        [HttpPost("tweezers-user/reset-password")]
        public ActionResult<bool> ResetPassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            if (!IdentityManager.UsingIdentity)
                return TweezersNotFound();

            return WrapWithAuthorizationCheck(() =>
            {
                JObject user =
                    IdentityManager.FindUserBySessionId(Request.Headers[IdentityManager.SessionIdKey], allFields: true);

                bool oldPasswordOk = ValidatePassword(changePasswordRequest.OldPassword, user["passwordHash"].ToString());
                if (!oldPasswordOk || changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
                {
                    return TweezersBadRequest("Passwords do not match");
                }

                if (changePasswordRequest.OldPassword == changePasswordRequest.NewPassword)
                {
                    return TweezersBadRequest("Old and new passwords are the same.");
                }

                return DoChangePassword(user, changePasswordRequest);
            }, "Reset Password", DefaultPermission.None);
        }

        private ActionResult DoChangePassword(JObject user, ChangePasswordRequest changePasswordRequest)
        {
            JObject passwordChange = new JObject()
            {
                ["passwordHash"] = Hash.Create(changePasswordRequest.NewPassword)
            };

            try
            {
                TweezersObject usersObjectMetadata =
                    TweezersSchemaFactory.Find(IdentityManager.UsersCollectionName, true);
                usersObjectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, user["_id"].ToString(),
                    passwordChange);

                return TweezersOk(TweezersGeneralResponse.Create("OK"));
            }
            catch
            {
                return TweezersBadRequest("Could not update password");
            }
        }

        private bool Authenticate(LoginRequest request, out JObject user)
        {
            user = FindUser(request.Username);

            if (user == null)
                return false;

            bool passwordValidated = ValidatePassword(request.Password, user["passwordHash"].ToString());

            if (!passwordValidated)
                user = null;

            return passwordValidated;
        }

        public static JObject FindUser(string username)
        {
            FindOptions<JObject> userOpts = new FindOptions<JObject>()
            {
                Predicate = (u) => u["username"].ToString().Equals(username),
                Take = 1
            };

            TweezersObject usersObjectMetadata =
                TweezersSchemaFactory.Find(IdentityManager.UsersCollectionName, true, true);
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, userOpts, true)?.Items
                .SingleOrDefault();
        }

        private bool ValidatePassword(string requestPassword, string userPasswordHash)
        {
            return Hash.Validate(requestPassword, userPasswordHash);
        }
    }
}