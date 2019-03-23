using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Controllers;
using Tweezers.Api.Database;
using Tweezers.Api.DataHolders;
using Tweezers.Api.Exceptions;
using Tweezers.Discoveries.Exceptions;
using Tweezers.Identity.DataHolders;

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

        [HttpPost("users")]
        public ActionResult<User> Post([FromBody] LoginRequest suggestedUser)
        {
            try
            {
                if (FindUser(suggestedUser) != null)
                {
                    return Forbid("username already taken");
                }

                User user = DataHolders.User.CreateUser(suggestedUser);
                return Ok(DatabaseProxy.Add(user.Id ,user));
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
            if (Authenticate(request))
            {
                return Ok("Welcome");
            }
            else
            {
                return Unauthorized("bad username or password");
            }
        }

        public bool Authenticate(LoginRequest request)
        {
            User user = FindUser(request);
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
