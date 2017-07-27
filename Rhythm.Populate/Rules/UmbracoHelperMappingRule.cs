﻿using System;
using System.Reflection;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate.Rules
{
    public class UmbracoHelperMappingRule<TModel> : IMappingRule where TModel : class
    {
        private readonly Func<UmbracoHelper, IPublishedContent> _filter;
        private readonly string _propertyName;

        public UmbracoHelperMappingRule(string propertyName, Func<UmbracoHelper, IPublishedContent> filter)
        {
            _propertyName = propertyName;
            _filter = filter;
        }

        void IMappingRule.Execute(IMappingSession session, MappingOptions options, object model, Type type, object source)
        {
            var relatedContent = session.Map<TModel>(_filter(new UmbracoHelper(UmbracoContext.Current))).WithOptions(options).Single();

            var destProperty = type.GetProperty(_propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) ?? type.GetProperty(_propertyName);

            destProperty.SetValue(model, relatedContent);
        }
    }
}