using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Controllers;

namespace Tweezers.MetadataManagement
{
    [Route("api/metadata")]
    [ApiController]
    public sealed class MetadataController : TweezersControllerBase
    {
        [HttpGet]
        public ActionResult<TweezersMetadata> GetMetadata()
        {
            return TweezersOk(TweezersMetadata.Instance, "schema");
        }
    }
}