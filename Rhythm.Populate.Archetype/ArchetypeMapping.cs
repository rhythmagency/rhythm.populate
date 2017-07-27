namespace Rhythm.Populate.Archetype
{
    public class ArchetypeMapping<T> : MappingBase<T> where T : class
    {
        public ArchetypeRuleBuilder<T> Map { get; }

        public ArchetypeMapping()
        {
            Map = new ArchetypeRuleBuilder<T>(Rules);
        }

        protected void Alias(string dataTypeName, string propertyAlias)
        {
            Populate.RegisterType($"{dataTypeName}.{propertyAlias}", typeof(T));
        }
    }
}