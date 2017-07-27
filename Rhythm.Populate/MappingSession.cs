using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate
{
    public class MappingSession : IMappingSession
    {
        private readonly IDictionary<string, object> _cache;

        public MappingSession()
        {
            _cache = new Dictionary<string, object>();
        }

        public MappingExecutor<T> Map<T>(IPublishedContent content) where T : class
        {
            return Map<T>(new[] {content});
        }

        public MappingExecutor<T> Map<T>(IEnumerable<IPublishedContent> contents) where T : class
        {
            return new MappingExecutor<T>(this, contents);
        }

        public MappingExecutor<T> Map<T>(int id) where T : class
        {
            var content = new UmbracoHelper(UmbracoContext.Current).TypedContent(id);

            return Map<T>(content);
        }

        public MappingExecutor<T> Map<T>() where T : class
        {
            var type = typeof(T);
            var mapping = Populate.GetMappingForType(type) as IContentMapping;

            if (mapping == null)
            {
                throw new Exception($"Type {type.FullName} does not implement IContentMapper");
            }

            if (mapping.ContentSource == null)
            {
                throw new Exception($"No mapping content source defined for type: {type}");
            }

            var contents = mapping.ContentSource(new UmbracoHelper(UmbracoContext.Current));

            return Map<T>(contents);
        }

        public bool Contains(string key)
        {
            return _cache.ContainsKey(key);
        }

        public T Get<T>(string key) where T : class
        {
            if (!Contains(key))
            {
                throw new Exception($"Content with key {key} has not been mapped");
            }

            var item = _cache[key] as T;

            if (item == null)
            {
                throw new Exception($"Content with key {key} could not be mapped to type {typeof(T).FullName}");
            }

            return item;
        }

        public void Add(string key, object model)
        {
            _cache.Add(key, model);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}