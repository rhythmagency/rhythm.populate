namespace Rhythm.Populate
{
    public class ComponentMapping<T> : MappingBase<T> where T : class
    {
        public ContentMappingRuleBuilder<T> Map { get; }

        public ComponentMapping()
        {
            Map = new ContentMappingRuleBuilder<T>(Rules);
        }
    }
}