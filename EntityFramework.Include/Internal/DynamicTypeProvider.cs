using System;

namespace EntityFramework.Include.Internal
{
    internal class DynamicTypeProvider : CacheableProvider<Type, Type>
    {
        internal static Type Provide<T>()
        {
            var target = typeof (T);
            if (ExistCache(target))
            {
                return FromCache(target);
            }

            var builded = DynamicTypeBuilder.Build(target);
            AddCache(target, builded);

            return builded;
        }
    }
}
