using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Controllers;

namespace Tweezers.MetadataManagement
{
    [Route("api/metadata")]
    [ApiController]
    public class MetadataController : TweezersControllerBase
    {
        [HttpGet]
        public ActionResult<TweezersMetadata> GetMetadata()
        {
            return TweezersOk(TweezersMetadata.Instance);
        }
    }
}