using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Shoe Brand", AllowedActions = TweezersAllowedActions.Admin, IconName = "all_inclusive")]
    public class ShoeBrand
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Full name")]
        public string Name { get; set; }
    }
}
