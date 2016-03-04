using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFramework.Include.Internal.Expressions
{
    internal class ExpressionHelper
    {
        internal static MemberExpression PathToExpression(Type target, string path)
        {
            var property = target.GetProperty(path);
            if (property == null)
            {
                throw new ArgumentException($"{path} is not a property of {target.FullName}", nameof(path));
            }

            return Expression.MakeMemberAccess(Expression.Parameter(target), property);
        }

        internal static bool CheckMemberAccess(Type target, Expression propertySelector)
        {
            if (propertySelector.NodeType != ExpressionType.MemberAccess)
            {
                return false;
            }

            var memberAccess = (MemberExpression) propertySelector;
            return memberAccess.Member.DeclaringType == target;
        }

        internal static IEnumerable<Tuple<MemberExpression, Expression>> DistinctPropertyAccessorPairs(
            IEnumerable<Tuple<MemberExpression, Expression>> pairs)
        {
            return pairs.Distinct(new PropertyAccessorComparer());
        }

        class PropertyAccessorComparer : IEqualityComparer<Tuple<MemberExpression, Expression>>
        {
            public bool Equals(Tuple<MemberExpression, Expression> x, Tuple<MemberExpression, Expression> y)
            {
                return x.Item1.Member.Name == y.Item1.Member.Name;
            }

            public int GetHashCode(Tuple<MemberExpression, Expression> obj)
            {
                return obj.Item1.Member.Name.GetHashCode();
            }
        }
    }
}
