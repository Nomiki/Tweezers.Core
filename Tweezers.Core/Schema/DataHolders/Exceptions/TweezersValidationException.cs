using System;

namespace Tweezers.Schema.DataHolders.Exceptions
{
    public class TweezersValidationException : Exception
    {
        private readonly TweezersValidationResult _result;

        public TweezersValidationException(TweezersValidationResult result)
        {
            _result = result;
        }

        public override string Message => _result.ToString();
    }
}
