using System;

namespace Tweezers.Discoveries.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class TweezersFieldAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public string[] Values { get; set; } = null;

        public TweezersFieldAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
