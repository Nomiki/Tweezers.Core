using System;

namespace Tweezers.Discoveries.Attributes
{
    [AttributeUsage(validOn:AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class TweezersIgnoreAttribute : Attribute
    {

    }
}
