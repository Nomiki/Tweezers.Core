using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Tweezers.Discoveries.Attributes;

namespace Tweezers.Api.Controllers
{
    public abstract class TweezersControllerBase : ControllerBase
    { 
        internal PropertyInfo DetermineIdAttr<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any());
        }
    }
}
