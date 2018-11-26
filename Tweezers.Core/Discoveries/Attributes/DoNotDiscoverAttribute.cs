using System;

namespace Discoveries.Attributes
{
    [AttributeUsage(validOn:AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class DoNotDiscoverAttribute : Attribute
    {

    }
}
