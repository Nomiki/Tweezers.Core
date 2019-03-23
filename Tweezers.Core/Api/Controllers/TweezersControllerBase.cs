using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Api.Interfaces;
using Tweezers.Discoveries.Attributes;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    {
        protected IDatabaseProxy DatabaseProxy { get; set; }

        public PropertyInfo DetermineIdAttr<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any());
        }
    }
}
