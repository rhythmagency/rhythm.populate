using System;
using System.Reflection;
using Archetype.Models;
using Umbraco.Web;

namespace Rhythm.Populate.Archetype.Rules
{
    public class ArchetypeNodeMappingRule<TModel> : IMappingRule where TModel : class
    {
        private readonly string _propertyAlias;
        private readonly string _propertyName;
        private bool _isMedia;

        public ArchetypeNodeMappingRule(string propertyName, string propertyAlias)
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

            var srcValue = fieldset.GetValue<string>(_propertyAlias);

            var helper = new UmbracoHelper(UmbracoContext.Current);

            var node = _isMedia ? helper.TypedMedia(srcValue) : helper.TypedContent(srcValue);

            if (node == null)
            {
                //Orphan node reference, so ignore
                return;
            }

            var mappedNode = session.Map<TModel>(node).WithOptions(options).Single();

            destProperty.SetValue(model, mappedNode);
        }

        public void AsMedia()
        {
            _isMedia = true;
        }
    }
}