using System;
using System.Linq;
using System.Reflection;
using Archetype.Models;
using Umbraco.Web;

namespace Rhythm.Populate.Archetype.Rules
{
    public class ArchetypeNodeCollectionMappingRule<TModel> : IMappingRule where TModel : class
    {
        private readonly string _propertyAlias;
        private readonly string _propertyName;
        private bool _isLazy;
        private bool _isMedia;

        public ArchetypeNodeCollectionMappingRule(string propertyName, string propertyAlias)
        {
            _propertyName = propertyName;
            _propertyAlias = propertyAlias;
            _isLazy = true;
        }

        public void Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
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


            if (_isLazy && (!options.IncludedProperties.ContainsKey(type) || !options.IncludedProperties[type].Contains(_propertyName)))
            {
                return;
            }

            var srcValue = fieldset.GetValue<string>(_propertyAlias);

            var srcNodes = srcValue.Split(',');

            var helper = new UmbracoHelper(UmbracoContext.Current);

            var list = srcNodes.Select(x =>
                               {
                                   var node = _isMedia ? helper.TypedMedia(x) : helper.TypedContent(x);

                                   return session.Map<TModel>(node).WithOptions(options).Single();
                               }).ToList();

            destProperty.SetValue(model, list);
        }

        public ArchetypeNodeCollectionMappingRule<TModel> AsMedia()
        {
            _isMedia = true;
            return this;
        }

        public ArchetypeNodeCollectionMappingRule<TModel> Eager()
        {
            _isLazy = false;
            return this;
        }
    }
}