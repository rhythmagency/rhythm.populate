using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Rhythm.Populate
{
    public static class Populate
    {
        private const string CURRENT_SESSION_KEY = "Rhythm.Populate:CurrentSession";
        private static readonly IDictionary<Type, IMapping> _mappings = new Dictionary<Type, IMapping>();
        private static readonly IDictionary<string, Type> _aliases = new Dictionary<string, Type>();

        static Populate()
        {
            IncludeBuiltInMappings();
        }

        private static void IncludeBuiltInMappings()
        {
            _mappings.Add(typeof(PublishedContentBaseModel), new PublishedContentBaseModelMap());
        }

        public static void RegisterType(string alias, Type type)
        {
            _aliases.Add(alias, type);
        }

        public static Type GetRegisteredType(string alias, Type defaultType)
        {
            var type = !_aliases.ContainsKey(alias) ? null : _aliases[alias];

            if (type == null)
            {
                return defaultType;
            }

            if (type == defaultType)
            {
                return defaultType;
            }

            if (!defaultType.IsAssignableFrom(type))
            {
                return defaultType;
            }

            return type;
        }

        public static IMapping GetMappingForType(Type type)
        {
            return !_mappings.ContainsKey(type) ? null : _mappings[type];
        }

        public static void AddMappingsFromAssemblyOf<T>()
        {
            AddMappingsFromAssembly(typeof(T).Assembly);
        }

        public static void AddMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => typeof(IMapping).IsAssignableFrom(t)).ToList();

            foreach (var type in types)
            {
                var mapping = Activator.CreateInstance(type) as IMapping;

                if (mapping == null)
                {
                    throw new Exception($"Type: {type.FullName} could not be converted to IMapping");
                }

                if (_mappings.ContainsKey(mapping.Type))
                {
                    throw new Exception($"A mapping for type: {mapping.Type} already exists");
                }

                _mappings.Add(mapping.Type, mapping);
            }
        }

        public static IMappingSession CreateSession()
        {
            return new MappingSession();
        }

        public static IMappingSession GetCurrentSessionForRequest()
        {
            if (HttpContext.Current == null)
            {
                throw new Exception("Could not access HttpContext");
            }

            if (HttpContext.Current.Items.Contains(CURRENT_SESSION_KEY))
            {
                return HttpContext.Current.Items[CURRENT_SESSION_KEY] as IMappingSession;
            }

            HttpContext.Current.Items[CURRENT_SESSION_KEY] = new MappingSession();

            return (IMappingSession) HttpContext.Current.Items[CURRENT_SESSION_KEY];
        }
    }
}