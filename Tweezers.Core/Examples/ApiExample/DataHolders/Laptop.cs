using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Laptop", AllowedActions = TweezersAllowedActions.Admin, IconName = "laptop")]
    public class Laptop
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Brand Name")]
        public string BrandName { get; set; }

        [TweezersField("Model")]
        public string Name { get; set; }

        [TweezersField("CPU Model")]
        public string CpuModel { get; set; }

        [TweezersField("Memory")]
        public string Memory { get; set; }

        [TweezersField("Disk")]
        public string Disk { get; set; }

        [TweezersField("Price")]
        public double Price { get; set; }
    }
}