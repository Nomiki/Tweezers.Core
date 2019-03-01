using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.DataHolders;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Engine;

namespace Tweezers.Api.Controllers
{
    [Route("api/tweezers")]
    [ApiController]
    public abstract class TweezersEntryPoint : ControllerBase
    {
        protected static string Title { get; set; }
        protected static string Description { get; set; }

        private static List<SideMenuData> classList;

        [HttpGet]
        public ActionResult<List<DiscoverableMetadata>> Discover()
        {
            if (classList == null)
            {
                classList = Assembly.GetEntryAssembly().GetExportedTypes()
                    .Where(t => t.IsSubclassOf(typeof(TweezersControllerBase)))
                    .Select(FromController)
                    .ToList();
            }

            return Ok(classList);
        }

        public static SideMenuData FromController(Type controller)
        {
            if (controller.BaseType.GenericTypeArguments.Any())
            {
                Type tweezersEntity = FindTweezersEntity(controller);
                TweezersEntityAttribute tweezersEntityAttributeData = GetTweezersEntityAttributeData(tweezersEntity);

                return new SideMenuData()
                {
                    Name = controller.Name,
                    Description = controller.FullName,
                    DisplayName = tweezersEntityAttributeData.DisplayName,
                    ReferenceLink = $"{controller.Name.ToLower().Replace("controller", "")}",
                    IconName = tweezersEntityAttributeData.IconName
                };
            }

            //TODO: maybe even throw
            return null;
        }

        private static TweezersEntityAttribute GetTweezersEntityAttributeData(Type tweezersEntity)
        {
            return tweezersEntity.GetCustomAttributes<TweezersEntityAttribute>().Single();
        }

        private static Type FindTweezersEntity(Type controller)
        {
            return controller.BaseType.GenericTypeArguments.Single(t => t.GetCustomAttributes<TweezersEntityAttribute>().Any());
        }
    }
}