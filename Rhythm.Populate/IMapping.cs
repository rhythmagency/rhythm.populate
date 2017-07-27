using System;
using System.Collections.Generic;

namespace Rhythm.Populate
{
    public interface IMapping
    {
        Type Type { get; }
        IEnumerable<IMappingRule> GetRules();
        IMappingRule GetRuleByProperty(string propertyName);
    }
}