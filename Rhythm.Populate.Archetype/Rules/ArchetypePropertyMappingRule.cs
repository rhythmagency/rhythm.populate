using System;
using System.Reflection;
using Archetype.Models;

namespace Rhythm.Populate.Archetype.Rules
{
    public class ArchetypePropertyMappingRule<T> : IMappingRule
    {
        private readonly string _propertyAlias;
        private readonly string _propertyName;

        public ArchetypePropertyMappingRule(string propertyName, string propertyAlias)
        {
            _propertyName = propertyName;
            _propertyAlias = propertyAlias;
        }

        void IMappingRule.Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
        {
            var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

            var fieldset = source as ArchetypeFieldsetModel;

            if (fieldset == null)
            {
                return;
            }

            if (!fieldset.HasProperty(_propertyAlias) || !fieldset.HasValue(_propertyAlias))
            {
                return;
            }

            var srcValue = fieldset.GetValue<T>(_propertyAlias);

            destProperty.SetValue(model, srcValue);
        }
    }
}