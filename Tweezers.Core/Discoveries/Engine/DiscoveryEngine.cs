using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Common;
using Tweezers.Discoveries.Containers;
using Tweezers.Discoveries.Exceptions;

namespace Tweezers.Discoveries.Engine
{
    public static class DiscoveryEngine
    {
        private static readonly Dictionary<Type, DiscoverableMetadata> calculatedMetadata = new Dictionary<Type, DiscoverableMetadata>();
        private static readonly object cacheLock = new object();

        private static bool ShouldDiscover(Type clazz)
        {
            return clazz.GetCustomAttributes<TweezersEntityAttribute>().Any();
        }

        private static DiscoverableMetadata DiscoverClass(Type clazz)
        {
            return new DiscoverableMetadata()
            {
                Name = clazz.FullName,
                EntityData = clazz.GetCustomAttributes<TweezersEntityAttribute>().Single(),
                PropertyData = clazz.GetProperties()
                    .Where(p => p.GetCustomAttributes<TweezersIgnoreAttribute>().None())
                    .Select(p => new PropertyMetadata(p))
                    .ToList()
            };
        }

        public static DiscoverableMetadata GetData(Type clazz)
        {
            lock (cacheLock)
            {
                if (calculatedMetadata.ContainsKey(clazz))
                    return calculatedMetadata[clazz];

                if (ShouldDiscover(clazz))
                {
                    DiscoverableMetadata metadata = DiscoverClass(clazz);
                    calculatedMetadata[clazz] = metadata;
                    return metadata;
                }

                throw new TweezersDiscoveryException(clazz);
            }
        }


    }
}