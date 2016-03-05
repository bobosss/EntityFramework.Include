using System;
using EntityFramework.Include.Internal.Expressions;

namespace EntityFramework.Include.Internal
{
    internal class DelegateProvider : CacheableProvider<Tuple<Type, Type>, Func<object, object>>
    {
        internal static Func<object, object> Shift(Type source, Type result)
        {
            var tuple = Tuple.Create(source, result);
            if (ExistCache(tuple))
            {
                return FromCache(tuple);
            }

            var f = new ExpressionBuilder().Shift(source, result).Compile();
            AddCache(tuple, f);

            return f;
        }
    }
}
