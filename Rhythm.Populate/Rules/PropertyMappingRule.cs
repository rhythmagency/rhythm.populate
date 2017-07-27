using System;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate.Rules
{
    public class PropertyMappingRule<T> : IMappingRule
    {
        private readonly string _propertyAlias;
        private readonly string _propertyName;
        private Func<T, T> _transform;

        public PropertyMappingRule(string propertyName, string propertyAlias)
        {
            _propertyName = propertyName;
            _propertyAlias = propertyAlias;
        }

        void IMappingRule.Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
        {
            var content = source as IPublishedContent;

            if (content == null)
            {
                throw new Exception("Expected source type IPublishedContent");
            }

            var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

            var srcValue = content.GetPropertyValue<T>(_propertyAlias);

            if (_transform != null)
            {
                srcValue = _transform(srcValue);
            }

            destProperty.SetValue(model, srcValue);
        }

        public void Transform(Func<T, T> transform)
        {
            _transform = transform;
        }
    }
}