using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Product", AllowedActions = TweezersAllowedActions.Admin, IconName = "shopping_cart")]
    public class Product
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Full name")]
        public string Name { get; set; }
    }
}
