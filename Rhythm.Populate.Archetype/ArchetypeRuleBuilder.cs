using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Rhythm.Populate.Archetype.Rules;

namespace Rhythm.Populate.Archetype
{
    public class ArchetypeRuleBuilder<T> : RuleBuilderBase where T : class
    {
        public ArchetypeRuleBuilder(IDictionary<string, IMappingRule> rules) : base(rules) {}

        public void Property<TModel>(Expression<Func<T, TModel>> property)
        {
            Property(property, null);
        }

        public void Property<TModel>(Expression<Func<T, TModel>> property, string alias)
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new ArchetypePropertyMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);
        }

        public ArchetypeNodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property) where TModel : class
        {
            return Node(property, null);
        }

        public ArchetypeNodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new ArchetypeNodeMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);

            return rule;
        }

        public ArchetypeNodeCollectionMappingRule<TModel> NodeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property) where TModel : class
        {
            return NodeCollection(property, null);
        }

        public ArchetypeNodeCollectionMappingRule<TModel> NodeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new ArchetypeNodeCollectionMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);

            return rule;
        }

        public void Archetype<TModel>(Expression<Func<T, TModel>> property) where TModel : class
        {
            Archetype(property, null);
        }

        public void Archetype<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            alias = alias ?? member.Name.ToCamelCase();

            AddRule(member.Name, new ArchetypeMappingRule<TModel>(member.Name, alias));
        }

        public void ArchetypeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property) where TModel : class
        {
            ArchetypeCollection(property, null);
        }

        public void ArchetypeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            alias = alias ?? member.Name.ToCamelCase();

            AddRule(member.Name, new ArchetypeCollectionMappingRule<TModel>(member.Name, alias));
        }
    }
}