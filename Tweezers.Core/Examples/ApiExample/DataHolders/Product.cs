using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Product", AllowedActions = TweezersAllowedActions.Admin)]
    public class Product
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Full name")]
        public string Name { get; set; }
    }
}
