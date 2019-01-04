using Tweezers.Discoveries.Attributes;
using Tweezers.Discoveries.Rbac;

namespace ApiExample.DataHolders
{
    [TweezersEntity("Person", AllowedActions = TweezersAllowedActions.Admin)]
    public class Person
    {
        [TweezersId]
        public string Id { get; set; }

        [TweezersField("Full name")]
        public string Name { get; set; }
        
        [TweezersField("Cat name")]
        public string CatName { get; set; }

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
