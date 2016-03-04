using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.Include.Internal.Expressions
{
    internal class ExpressionBuilder
    {

        internal Expression<Func<object, object>> Shift(Type source, Type result)
        {
            var param = Expression.Parameter(typeof(object));
            var init = MakeMemberInit(source, result, param);

            return Expression.Lambda<Func<object, object>>(init, param);
        }

        internal Expression<Func<TSource, object>> ShiftWith<TSource>(Type result, IEnumerable<Tuple<MemberExpression, Expression>> accessorPairs)
        {
            var source = typeof(TSource);
            var param = Expression.Parameter(source);

            var accessorDic = accessorPairs.ToDictionary(_ => _.Item1.Member.Name, _ => _);
            var init = MakeMemberInit(source, result, param, accessorDic);

            return Expression.Lambda<Func<TSource, object>>(init, param);
        }

        private MemberInitExpression MakeMemberInit(Type source, Type result, ParameterExpression param, 
            Dictionary<string, Tuple<MemberExpression, Expression>> accessorDic = null)
        {
            if (accessorDic == null)
            {
                accessorDic = new Dictionary<string, Tuple<MemberExpression, Expression>>();
            }
            
            var visiter = new ParameterVisitor(param);
            
            var cotr = Expression.New(result);
            var bind = source.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(_ =>
                {
                    if (accessorDic.ContainsKey(_.Name))
                    {
                        var accessor = accessorDic[_.Name].Item1;
                        var s = accessorDic[_.Name].Item2;
                        var setter = visiter.Visit(s);

                        return Expression.Bind(accessor.Member, setter);
                    }

                    return Expression.Bind(result.GetProperty(_.Name),
                        visiter.Visit(Expression.MakeMemberAccess(MakeConvert(param, source), _)));
                });

            return Expression.MemberInit(cotr, bind);
        }

        private Expression MakeConvert(Expression source, Type resultType)
        {
            return Expression.Convert(source, resultType);
        }
    }
}
