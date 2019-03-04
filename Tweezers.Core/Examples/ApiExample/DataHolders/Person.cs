using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Person", AllowedActions = TweezersAllowedActions.Admin, IconName = "person")]
    public class Person
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Full name")]
        public string Name { get; set; }

        [TweezersField("Gender")]
        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Unknown
    }
}
