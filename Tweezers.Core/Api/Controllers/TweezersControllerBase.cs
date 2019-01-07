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
        private static readonly Dictionary<Type, PropertyInfo> IdPropertyMap;

        static TweezersControllerBase()
        {
            IdPropertyMap = new Dictionary<Type, PropertyInfo>();
        }

        internal PropertyInfo GetIdProperty<T>()
        {
            return IdPropertyMap.ContainsKey(typeof(T))
                ? IdPropertyMap[typeof(T)]
                : (IdPropertyMap[typeof(T)] = DetermineIdAttr<T>());
        }

        private PropertyInfo DetermineIdAttr<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            return properties.Single(pi => pi.GetCustomAttributes<TweezersIdAttribute>().Any());
        }
    }
}
