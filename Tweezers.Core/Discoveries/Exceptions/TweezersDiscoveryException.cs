using System;

namespace Tweezers.Discoveries.Exceptions
{
    public sealed class TweezersDiscoveryException : Exception
    {
        public TweezersDiscoveryException(Type type)
            : base($"Could not discover type {type.FullName}.")
        {
        }
    }
}
