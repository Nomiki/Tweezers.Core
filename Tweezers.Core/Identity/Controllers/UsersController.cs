using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Controllers;
using Tweezers.Api.Database;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Exceptions;
using Tweezers.Discoveries.Exceptions;
using Tweezers.Identity.DataHolders;
using Tweezers.Discoveries.Common;

namespace Tweezers.Identity.Controllers
{
    [Route("api")]
    [ApiController]
    public class UsersController : TweezersControllerBase
    {
        public UsersController()
        {
            this.DatabaseProxy = LocalDatabase.Instance;
        }

        protected TimeSpan SessionTimeout => 4.Hours();

        [HttpPost("users")]
        public ActionResult<User> Post([FromBody] LoginRequest suggestedUser)
        {
            try
            {
                if (FindUser(suggestedUser) != null)
                {
                    return ForbiddenResult("post", "user");
                }

                User user = DataHolders.User.CreateUser(suggestedUser);
                return Ok(DeleteTweezersIgnores(DatabaseProxy.Add(user.Id ,user)));
            }
            catch (TweezersDiscoveryException)
            {
                return Forbid("tweezers", typeof(User).Name);
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginRequest request)
        {
            User user;
            if (Authenticate(request, out user))
            {
                user.SessionId = Guid.NewGuid().ToString();
                user.SessionExpiry = DateTime.Now + SessionTimeout;
                user.PasswordHash = null;
                DatabaseProxy.Edit<User>(user.Id, user);
                return Ok("Welcome");
            }
            else
            {
                return Unauthorized("bad username or password");
            }
        }

        public bool Authenticate(LoginRequest request, out User user)
        {
            try
            {
                user = FindUser(request);
            }
            catch (ItemNotFoundException e)
            {
                user = null;
            }

            if (user == null)
                return false;

            return user.ValidatePassword(request.Password);
        }

        private User FindUser(LoginRequest request)
        {
            FindOptions<User> userOpts = new FindOptions<User>()
            {
                Predicate = (u) => u.Username.Equals(request.Username),
                Take = 1
            };

            return DatabaseProxy.List(userOpts)?.SingleOrDefault();
        }
    }
}
