using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected ActionResult ForbiddenResult(string method, string message)
        {
            return StatusCode(403, new TweezersErrorBody() { Message = message, Method = method});
        }

        protected ActionResult NotFoundResult(string message)
        {
            return StatusCode(404, new TweezersErrorBody() {Message = message});
        }

        protected ActionResult BadRequestResult(string message)
        {
            return StatusCode(400, new TweezersErrorBody() { Message = message });
        }

        protected ActionResult UnauthorizedResult(string message)
        {
            return StatusCode(401, new TweezersErrorBody() { Message = message });
        }
    }
}
