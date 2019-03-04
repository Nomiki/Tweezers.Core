using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Peripheral", AllowedActions = TweezersAllowedActions.Admin, IconName = "mouse")]
    public class Peripheral
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Brand Name")]
        public string BrandName { get; set; }

        [TweezersField("Model")]
        public string Name { get; set; }

        [TweezersField("Type")]
        public PeripheralType PeripheralType { get; set; }

        [TweezersField("Price")]
        public double Price { get; set; }
    }

    public enum PeripheralType
    {
        Mouse,
        Keyboard,
        Screen,
        Speakers,
        Headphones
    }
}
