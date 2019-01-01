using System;
using Tweezers.Discoveries.Rbac;

namespace Tweezers.Discoveries.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TweezersEntityAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public TweezersAllowedActions AllowedActions { get; set; } = TweezersAllowedActions.None;

        public TweezersEntityAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
