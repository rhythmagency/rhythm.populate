using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate.Rules
{
    public class NodeCollectionMappingRule<TModel> : IMappingRule where TModel : class
    {
        private readonly string _propertyAlias;
        private readonly string _propertyName;
        private bool _isLazy;
        private bool _isMedia;

        public NodeCollectionMappingRule(string propertyName, string propertyAlias)
        {
            _propertyName = propertyName;
            _propertyAlias = propertyAlias;
            _isLazy = true;
        }

        public void Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
        {
            var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

            var content = source as IPublishedContent;

            if (content == null)
            {
                return;
            }

            if (_isLazy && (!options.IncludedProperties.ContainsKey(type) || !options.IncludedProperties[type].Contains(_propertyName)))
            {
                return;
            }

            var srcProperty = content.GetProperty(_propertyAlias);

            if ((srcProperty == null) || !srcProperty.HasValue)
            {
                return;
            }

            var srcValue = srcProperty.Value as string;

            var srcNodes = srcValue.Split(',');

            var helper = new UmbracoHelper(UmbracoContext.Current);

            var list = srcNodes.Select(x =>
                               {
                                   var node = _isMedia ? helper.TypedMedia(x) : helper.TypedContent(x);

                                   return node == null ? null : session.Map<TModel>(node).WithOptions(options).Single();
                               }).Where(n => n != null).ToList();

            destProperty.SetValue(model, list);
        }

        public NodeCollectionMappingRule<TModel> AsMedia()
        {
            _isMedia = true;
            return this;
        }

        public NodeCollectionMappingRule<TModel> Eager()
        {
            _isLazy = false;
            return this;
        }
    }
}