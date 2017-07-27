using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Rhythm.Populate
{
    public static class ReflectionHelper
    {
        public static MemberInfo ToMember(this Expression expression)
        {
            var memberExpression = GetMemberExpression(expression);

            return memberExpression?.Member;
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            MemberExpression memberExpression = null;

            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    var body = (UnaryExpression) expression;
                    memberExpression = body.Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = expression as MemberExpression;
                    break;
            }

            return memberExpression;
        }

        public static IList<Type> GetHierarchy(this Type type)
        {
            var types = new List<Type> {type};

            var parentType = type.BaseType;

            while (parentType != null)
            {
                types.Add(parentType);
                parentType = parentType.BaseType;
            }

            types.Reverse();

            types.AddRange(type.GetInterfaces());

            return types;
        }
    }
}