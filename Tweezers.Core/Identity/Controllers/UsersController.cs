using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Controllers;
using Tweezers.Identity.DataHolders;
using Tweezers.Identity.HashUtils;
using Tweezers.Schema.Common;
using Tweezers.Schema.DataHolders;
using Tweezers.Schema.DataHolders.DB;
using Tweezers.Schema.DataHolders.Exceptions;
using Tweezers.Schema.Exceptions;

namespace Tweezers.Identity.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : TweezersControllerBase
    {
        protected TimeSpan SessionTimeout => 4.Hours();

        [HttpPost("users")]
        public ActionResult<JObject> Post([FromBody] LoginRequest suggestedUser)
        {
            try
            {
                if (FindUser(suggestedUser) != null)
                {
                    throw new TweezersValidationException(TweezersValidationResult.Reject($"Unable to create user"));
                }

                JObject user = new JObject
                {
                    ["username"] = suggestedUser.Username,
                    ["passwordHash"] = Hash.Create(suggestedUser.Password)
                };

                TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find("users", true);
                return new ActionResult<JObject>(usersObjectMetadata.Create(TweezersSchemaFactory.DatabaseProxy, user));
            }
            catch (TweezersValidationException e)
            {
                return BadRequestResult(e.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (Authenticate(request, out JObject user))
                {
                    user["sessionId"] = Guid.NewGuid().ToString();
                    user["sessionExpiry"] = (DateTime.Now + SessionTimeout).ToUniversalTime()
                        .ToString(CultureInfo.InvariantCulture);
                    TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find("users", true);
                    usersObjectMetadata.Update(TweezersSchemaFactory.DatabaseProxy, user["_id"].ToString(), user.Just("sessionId", "sessionExpiry"));
                    return Ok("Welcome");
                }

                return UnauthorizedResult("Bad username or password");
            }
            catch (TweezersValidationException e)
            {
                return BadRequestResult(e.Message);
            }
        }

        private bool Authenticate(LoginRequest request, out JObject user)
        {
            user = FindUser(request);

            if (user == null)
                return false;

            bool passwordValidated = ValidatePassword(request.Password, user["passwordHash"].ToString());

            if (!passwordValidated)
                user = null;

            return passwordValidated;
        }

        private JObject FindUser(LoginRequest request)
        {
            FindOptions<JObject> userOpts = new FindOptions<JObject>()
            {
                Predicate = (u) => u["username"].ToString().Equals(request.Username),
                Take = 1
            };

            TweezersObject usersObjectMetadata = TweezersSchemaFactory.Find("users", true);
            return usersObjectMetadata.FindInDb(TweezersSchemaFactory.DatabaseProxy, userOpts)?.SingleOrDefault();
        }

        public bool ValidatePassword(string requestPassword, string userPasswordHash)
        {
            return Hash.Validate(requestPassword, userPasswordHash);
        }
    }
}
