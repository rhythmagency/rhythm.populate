using Rhythm.Populate.Archetype.Extensions;

namespace Rhythm.Populate.Test
{
    public class TestMap : ContentMapping<TestModel>
    {
        public TestMap()
        {
            Alias("Foo");

            Map.Property(x => x.StringProperty);
            Map.Archetype(x => x.StringProperty);
        }
    }
}