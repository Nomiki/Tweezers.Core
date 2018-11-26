using Discoveries.Attributes;

namespace ExampleProject
{
    [Discoverable("Student")]
    public class DataClass
    {
        [DoNotDiscover]
        public int Id { get; set; }

        [DisplayName("Student Name")]
        public string Name { get; set; }
    }
}
