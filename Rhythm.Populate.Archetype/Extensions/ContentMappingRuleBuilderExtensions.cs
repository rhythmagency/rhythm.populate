using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rhythm.Populate.Archetype.Rules;

namespace Rhythm.Populate.Archetype.Extensions
{
    public static class ContentMappingRuleBuilderExtensions
    {
        public static void Archetype<TModel, T>(this ContentMappingRuleBuilder<T> ruleBuilder, Expression<Func<T, TModel>> property) where TModel : class where T : class
        {
            ruleBuilder.Archetype(property, null);
        }

        public static void Archetype<TModel, T>(this ContentMappingRuleBuilder<T> ruleBuilder, Expression<Func<T, TModel>> property, string alias) where TModel : class where T : class
        {
            var member = property.Body.ToMember();

            alias = alias ?? member.Name.ToCamelCase();

            ruleBuilder.AddRule(member.Name, new ArchetypeMappingRule<TModel>(member.Name, alias));
        }

        public static void ArchetypeCollection<TModel, T>(this ContentMappingRuleBuilder<T> ruleBuilder, Expression<Func<T, IEnumerable<TModel>>> property) where TModel : class where T : class
        {
            ruleBuilder.ArchetypeCollection(property, null);
        }

        public static void ArchetypeCollection<TModel, T>(this ContentMappingRuleBuilder<T> ruleBuilder, Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class where T : class
        {
            var member = property.Body.ToMember();

            alias = alias ?? member.Name.ToCamelCase();

            ruleBuilder.AddRule(member.Name, new ArchetypeCollectionMappingRule<TModel>(member.Name, alias));
        }
    }
}