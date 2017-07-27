using System;
using System.Collections.Generic;

namespace Rhythm.Populate
{
    public abstract class MappingBase<T> : IMapping
    {
        public Type Type { get; }
        protected IDictionary<string, IMappingRule> Rules { get; }

        protected MappingBase()
        {
            Type = typeof(T);
            Rules = new Dictionary<string, IMappingRule>();
        }

        public IEnumerable<IMappingRule> GetRules()
        {
            return Rules.Values;
        }

        public IMappingRule GetRuleByProperty(string propertyName)
        {
            if (!Rules.ContainsKey(propertyName))
            {
                throw new Exception($"Could not get rule for property name: {propertyName}");
            }

            return Rules[propertyName];
        }
    }
}