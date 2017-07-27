using System.Collections.Generic;

namespace Rhythm.Populate
{
    public abstract class RuleBuilderBase
    {
        private readonly IDictionary<string, IMappingRule> _rules;

        protected RuleBuilderBase(IDictionary<string, IMappingRule> rules)
        {
            _rules = rules;
        }

        public void AddRule(string propertyName, IMappingRule rule)
        {
            _rules.Add(propertyName, rule);
        }
    }
}