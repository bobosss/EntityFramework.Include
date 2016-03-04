using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityFramework.Include.Internal.Expressions
{
    internal class ParameterVisitor : ExpressionVisitor
    {
        private Dictionary<Type, ParameterExpression> Parameters { get; } = new Dictionary<Type, ParameterExpression>();

        internal ParameterVisitor(ParameterExpression parameter)
        {
            Parameters.Add(parameter.Type, parameter);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            return base.VisitParameter(Parameters.ContainsKey(parameter.Type)
                ? Parameters[parameter.Type]
                : parameter);
        }
    }
}
