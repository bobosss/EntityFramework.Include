using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Include.Utilities;

namespace EntityFramework.Include.Extensions
{
    public static class IncludeExtension
    {
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> queryable,
            string path,
            Expression<Func<T, IEnumerable<TProperty>>> query)
        {
            ArgumentNull.Check(path, nameof(path));
            ArgumentNull.Check(query, nameof(query));

            var call = Expression.Call(null,
                        new Func<IQueryable<T>, string, Expression<Func<T, IEnumerable<TProperty>>>,
                        IQueryable<T>>(Include).Method, queryable.Expression, Expression.Constant(path), Expression.Constant(query));
            
            return CreateQuery(queryable, call);
        }

        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> queryable,
            Expression<Func<T, IEnumerable<TProperty>>> propertySelector,
            Expression<Func<T, IEnumerable<TProperty>>> query)
        {
            ArgumentNull.Check(propertySelector, nameof(propertySelector));
            ArgumentNull.Check(query, nameof(query));

            var call = Expression.Call(null,
                        new Func<IQueryable<T>, Expression<Func<T, IEnumerable<TProperty>>>, Expression<Func<T, IEnumerable<TProperty>>>,
                        IQueryable<T>>(Include).Method, queryable.Expression, Expression.Constant(propertySelector), Expression.Constant(query));

            return CreateQuery(queryable, call);
        }

        private static IQueryable<T> CreateQuery<T>(IQueryable<T> queryable, Expression expression)
        {
            return queryable.Provider.CreateQuery<T>(expression);
        }
    }
}
