using System;
using System.Collections.Generic;
using System.Text;

namespace Tweezers.Api.DataHolders
{
    public class TweezersGeneralResponse
    {
        public string Message { get; set; }

        public static TweezersGeneralResponse Create(string message)
        {
            return new TweezersGeneralResponse() {Message = message};
        }
    }
}
