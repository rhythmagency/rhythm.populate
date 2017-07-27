using System;
using System.Linq.Expressions;
using Rhythm.Populate.Rules;
using Umbraco.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;

namespace Rhythm.Populate
{
    public class ContentMappingRuleBuilder<T> : RuleBuilderBase where T : class
    {
        public ContentMappingRuleBuilder(IDictionary<string, IMappingRule> rules) : base(rules)
        {
        }

        public PropertyMappingRule<TModel> Property<TModel>(Expression<Func<T, TModel>> property)
        {
            return Property(property, null);
        }

        public PropertyMappingRule<TModel> Property<TModel>(Expression<Func<T, TModel>> property, string alias)
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new PropertyMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);

            return rule;
        }

        public void Property(Expression<Func<T, object>> name, Func<IPublishedContent, object> sourceProperty)
        {
            var member = name.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            var mapping = new CustomPropertyMappingRule(member.Name, sourceProperty);

            AddRule(member.Name, mapping);
        }

        public NodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property) where TModel : class
        {
            return Node(property, null);
        }

        public NodeMappingRule<TModel> Node<TModel>(Expression<Func<T, TModel>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new NodeMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);

            return rule;
        }

        public NodeCollectionMappingRule<TModel> NodeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property) where TModel : class
        {
            return NodeCollection(property, null);
        }

        public NodeCollectionMappingRule<TModel> NodeCollection<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string alias) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            alias = alias ?? member.Name.ToCamelCase();

            var rule = new NodeCollectionMappingRule<TModel>(member.Name, alias);

            AddRule(member.Name, rule);

            return rule;
        }

        public void Parent<TModel>(Expression<Func<T, TModel>> property) where TModel : class
        {
            Content(property, x => x.Parent);
        }

        public ContentCollectionMappingRule<TModel> Children<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, string childAlias) where TModel : class
        {
            return Children(property, new[] {childAlias});
        }

        public ContentCollectionMappingRule<TModel> Children<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, params string[] childAliases) where TModel : class
        {
            return Content(property, x => x.Children.Where(c => childAliases.Contains(c.DocumentTypeAlias)));
        }

        public void Content<TModel>(Expression<Func<T, TModel>> property, Func<IPublishedContent, IPublishedContent> filter) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            var rule = new ContentMappingRule<TModel>(member.Name, filter);

            AddRule(member.Name, rule);
        }

        public ContentCollectionMappingRule<TModel> Content<TModel>(Expression<Func<T, IEnumerable<TModel>>> property, Func<IPublishedContent, IEnumerable<IPublishedContent>> filter) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            var rule = new ContentCollectionMappingRule<TModel>(member.Name, filter);

            AddRule(member.Name, rule);

            return rule;
        }

        public void Content<TModel>(Expression<Func<T, TModel>> property, Func<UmbracoHelper, IPublishedContent> filter) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            var rule = new UmbracoHelperMappingRule<TModel>(member.Name, filter);

            AddRule(member.Name, rule);
        }

        public UmbracoHelperCollectionMappingRule<TModel> Content<TModel>(Expression<Func<T, TModel>> property, Func<UmbracoHelper, IEnumerable<IPublishedContent>> filter) where TModel : class
        {
            var member = property.Body.ToMember();

            if (member == null)
            {
                throw new Exception("param [property] must be a member expression");
            }

            var rule = new UmbracoHelperCollectionMappingRule<TModel>(member.Name, filter);

            AddRule(member.Name, rule);

            return rule;
        }

        public void Component<TModel>(Expression<Func<T, TModel>> property, Action<ComponentMapping<TModel>> action) where TModel : class, new()
        {
            var member = property.Body.ToMember();
            var mapper = new ComponentMapping<TModel>();

            action(mapper);

            AddRule(member.Name, new ComponentMappingRule<TModel>(member.Name, mapper));
        }

        public void Component<TModel>(Expression<Func<T, TModel>> property) where TModel : class, new()
        {
            var member = property.Body.ToMember();

            // Can't call GetMapperForType() here because the mapper might not be registered yet. So instead we'll look
            // it up at runtime when the ComponentMappingRule is executed
            AddRule(member.Name, new ComponentMappingRule<TModel>(member.Name));
        }
    }
}