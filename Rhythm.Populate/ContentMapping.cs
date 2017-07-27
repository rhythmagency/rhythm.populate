using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate
{
    public abstract class ContentMapping<T> : MappingBase<T>, IContentMapping where T : class
    {
        protected ContentMappingRuleBuilder<T> Map { get; }

        public Func<UmbracoHelper, IEnumerable<IPublishedContent>> ContentSource { get; private set; }

        protected ContentMapping()
        {
            Map = new ContentMappingRuleBuilder<T>(Rules);
        }

        protected void MapFrom(Func<UmbracoHelper, IEnumerable<IPublishedContent>> contentSource)
        {
            ContentSource = contentSource;
        }

        protected void Alias(string alias)
        {
            Populate.RegisterType(alias, typeof(T));
        }
    }
}