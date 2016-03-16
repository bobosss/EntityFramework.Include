using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFramework.Include.Extensions;

namespace EntityFramework.Include.Internal.Expressions
{
    internal class ReplaceIncludeVisitor<TResult> : ExpressionVisitor
    {

        private string MethodName { get; } = "Include";
        private Type DeclaringType { get; } = typeof(IncludeExtension);

        private List<MethodCallExpression> Expressions { get; } = new List<MethodCallExpression>();

        internal IReadOnlyList<MethodCallExpression> IncludeExpressions => Expressions;

        internal Type QueryableTypeAtFirst { get; private set; }

        internal Expression Replace(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                QueryableTypeAtFirst = node.Type;
            }

            if (node.Method.DeclaringType == DeclaringType && node.Method.Name == MethodName)
            {
                if (node.Type == typeof (IQueryable<TResult>))
                {
                    Expressions.Add(node);
                }

                node = MakePassThrough(node.Arguments[0]);
            }
            return base.VisitMethodCall(node);
        }

        private MethodCallExpression MakePassThrough(Expression sourceQueryable)
        {
            var source = sourceQueryable.Type.GenericTypeArguments[0];
            var selector = MakePassThroughSelector(source);
            var select = typeof(Queryable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(_ => _.Name == "Select")
                .Where(_ =>
                {
                    var p = _.GetParameters();
                    return p.Length == 2
                            && p[0].ParameterType.IsGenericType
                            && p[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                            && p[1].ParameterType.IsGenericType
                            && p[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>);
                }).First()
                .MakeGenericMethod(source, source);

            return Expression.Call(null, select, sourceQueryable, selector);
        }

        private LambdaExpression MakePassThroughSelector(Type source)
        {
            var param = Expression.Parameter(source);
            return Expression.Lambda(param, param);
        }
    }
}
