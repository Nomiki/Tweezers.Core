using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweezers.Api;
using Tweezers.Api.Startup;

namespace Tweezers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TweezersServerRunner.Start(args);
        }
    }
}
