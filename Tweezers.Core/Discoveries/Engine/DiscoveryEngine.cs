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
        private static Dictionary<Type, DiscoverableMetadata> calculatedMetadata = new Dictionary<Type, DiscoverableMetadata>();

        public static void Discover(Assembly assembly = null)
        {
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            Type[] discoverableClasses = assembly.GetTypes()
                .Where(ShouldDiscover)
                .ToArray();

            calculatedMetadata = discoverableClasses
                .Select(clazz => new KeyValuePair<Type, DiscoverableMetadata>(clazz, DiscoverClass(clazz)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private static bool ShouldDiscover(Type clazz)
        {
            return clazz.GetCustomAttributes<DiscoverableAttribute>().Any();
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

        public static DiscoverableMetadata GetData(Type clazz)
        {
            if (calculatedMetadata.ContainsKey(clazz))
                return calculatedMetadata[clazz];

            if (ShouldDiscover(clazz))
            {
                DiscoverableMetadata metadata = DiscoverClass(clazz);
                calculatedMetadata[clazz] = metadata;
                return metadata;
            }

            return null;
        }
    }
}