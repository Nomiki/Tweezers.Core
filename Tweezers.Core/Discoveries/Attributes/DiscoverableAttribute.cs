using System;

namespace Discoveries.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DiscoverableAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public DiscoverableAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
