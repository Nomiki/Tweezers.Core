namespace Tweezers.Schema.DataHolders
{
    public class TweezersValidationResult
    {
        public bool Valid { get; private set; }

        public string Reason { get; private set; }

        private TweezersValidationResult()
        {

        }

        public static TweezersValidationResult Accept()
        {
            return new TweezersValidationResult()
            {
                Valid = true,
                Reason = null
            };
        }

        public static TweezersValidationResult Reject(string reason)
        {
            return new TweezersValidationResult()
            {
                Valid = false,
                Reason = reason
            };
        }
    }
}
