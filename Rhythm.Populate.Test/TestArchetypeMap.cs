using Rhythm.Populate.Archetype;

namespace Rhythm.Populate.Test
{
    public class TestArchetypeMap : ArchetypeMapping<TestModel>
    {
        public TestArchetypeMap()
        {
            Alias("foo", "foo");
            Map.Property(x => x.StringProperty);
        }
    }
}