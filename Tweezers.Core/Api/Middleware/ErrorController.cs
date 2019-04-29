using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;

namespace Tweezers.Api.Middleware
{
    [Route("[controller]")]
    public class ErrorController : Controller
    {
        [Route("")]
        [AllowAnonymous]
        protected IActionResult Get()
        {
            TweezersErrorBody errorBody = new TweezersErrorBody() { Message = "Internal Server Error, Check logs" };

            return StatusCode(500, errorBody);
        }
    }
}
