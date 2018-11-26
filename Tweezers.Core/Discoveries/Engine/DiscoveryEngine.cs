using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discoveries.Attributes;
using Discoveries.Common;
using Discoveries.Containers;

namespace Discoveries.Engine
{
    public sealed class DiscoveryEngine
    {
        public static void Discover()
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            Type[] discoverableClasses = assembly.GetTypes()
                .Where(t => t.GetCustomAttributes<DiscoverableAttribute>().Any())
                .ToArray();

            DiscoverableMetadata[] classesMetadata = discoverableClasses.Select(DiscoverClass).ToArray();
        }

        private static DiscoverableMetadata DiscoverClass(Type clazz)
        {
            return new DiscoverableMetadata()
            {
                Name = clazz.FullName,
                DisplayName = clazz.GetCustomAttributes<DiscoverableAttribute>().Single().DisplayName,
                PropertyData = clazz.GetProperties()
                    .Where(p => p.GetCustomAttributes<DoNotDiscoverAttribute>().None())
                    .Select(p => new PropertyMetadata(p))
                    .ToList()
            };
        }
    }
}