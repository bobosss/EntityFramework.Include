using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFramework.Include.Internal;

namespace EntityFramework.Include.Extensions
{
    public static class LinqExtension
    {
        public static List<T> ToListWithInclude<T>(this IQueryable<T> queryable)
        {
            var linqWith = new LinqWithInclude<T>(queryable);
            return linqWith.ToList();
        }

        public static async Task<List<T>> ToListWithIncludeAsync<T>(this IQueryable<T> queryable)
        {
            var linqWith = new LinqWithInclude<T>(queryable);
            return await linqWith.ToListAsync();
        }

        public static T[] ToArrayWithInclude<T>(this IQueryable<T> queryable)
        {
            var linqWith = new LinqWithInclude<T>(queryable);
            return linqWith.ToArray();
        }

        public static async Task<T[]> ToArrayWithIncludeAsync<T>(this IQueryable<T> queryable)
        {
            var linqWith = new LinqWithInclude<T>(queryable);
            return await linqWith.ToArrayAsync();
        }
    }
}
