using System;
using System.Collections.Generic;
using System.Text;

namespace Tweezers.Api.DataHolders
{
    public sealed class TweezersErrorBody
    {
        public int? Code { get; set; }
        public string Message { get; set; }
        public string Method { get; set; }
    }
}
