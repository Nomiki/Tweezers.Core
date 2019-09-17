using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweezers.Api.Controllers;
using Tweezers.Schema.Common;

namespace Tweezers.Api.MetadataManagement
{
    [Route("api/metadata")]
    [ApiController]
    public sealed class MetadataController : TweezersControllerBase
    {
        /// <summary>
        /// Initializes a new DB connection - fresh Tweezers instance
        /// </summary>
        /// <param name="settings">Settings in the request</param>
        /// <returns>200 if the DB was successfully connected and tweezers is up and running</returns>
        [Route("initialize")]
        [HttpPost]
        public ActionResult<JObject> SetMetadata([FromBody] TweezersInternalSettings settings)
        {
            try
            {
                if (!TweezersRuntimeSettings.Instance.IsInitialized)
                {
                    TweezersInternalSettings.WriteToSettingsFile(settings);
                    TweezersInternalSettings.Init();
                    TweezersRuntimeSettings.Instance.IsInitialized = true;
                    TweezersRuntimeSettings.Instance.WriteToSettingsFile();
                    return TweezersOk();
                }
            }
            catch (Exception e)
            {
                return TweezersBadRequest($"Unable to connect to DB: {e.Message}");
            }

            return TweezersBadRequest("Invalid Settings File");
        }

        /// <summary>
        /// Retrieve the metadata of the current tweezers instance
        /// </summary>
        /// <returns>Metadata of the current instance</returns>
        [HttpGet]
        public ActionResult<JObject> StartupData()
        {
            JObject response = JObject.FromObject(TweezersRuntimeSettings.Instance).Without("Port");
            if (TweezersInternalSettings.Instance != null && TweezersInternalSettings.Instance.AppDetails != null)
            {
                response["Title"] = TweezersInternalSettings.Instance.AppDetails.Title;
            }

            return TweezersOk(response);
        } 
    }
}