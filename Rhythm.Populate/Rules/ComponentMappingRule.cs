using System;
using System.Reflection;
using Umbraco.Core.Models;

namespace Rhythm.Populate.Rules
{
    public class ComponentMappingRule<T> : IMappingRule where T : class, new()
    {
        private readonly string _propertyName;
        private IMapping _componentMapping;

        public ComponentMappingRule(string propertyName, IMapping componentMapping)
        {
            _propertyName = propertyName;
            _componentMapping = componentMapping;
        }

        public ComponentMappingRule(string propertyName) : this(propertyName, null) {}

        void IMappingRule.Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
        {
            var content = source as IPublishedContent;

            if (content == null)
            {
                throw new Exception("Expected source type IPublishedContent");
            }

            var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

            var componentModel = new T();

            //If component was not mapped inline, then we need to fetch it from the mapping registry
            if (_componentMapping == null)
            {
                _componentMapping = Populate.GetMappingForType(typeof(T));

                if (_componentMapping == null)
                {
                    throw new Exception($"No component mapper is defined for type: {typeof(T).FullName}");
                }
            }

            var rules = _componentMapping.GetRules();

            foreach (var rule in rules)
            {
                rule.Execute(session, options, componentModel, typeof(T), content);
            }

            destProperty.SetValue(model, componentModel);
        }
    }
}