using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Include.Internal.Expressions;

namespace EntityFramework.Include.Internal
{
    internal class LinqWithInclude<T>
    {
        private ReplaceIncludeVisitor<T> Visitor { get; } = new ReplaceIncludeVisitor<T>();

        private Type DynamicType { get; } = DynamicTypeProvider.Provide<T>();
        
        private IQueryable<T> ReplacedQueryable { get; }


        internal LinqWithInclude(IQueryable<T> queryable)
        {
            ReplacedQueryable = queryable.Provider.CreateQuery<T>(Visitor.Replace(queryable.Expression));
        }

        internal List<T> ToList()
        {
            if (ShouldShift())
            {
                var queryable = AddShiftDynamicTypeAtTail(ReplacedQueryable);
                return ShiftList(queryable.ToList());
            }
            return ReplacedQueryable.ToList();
        }

        internal async Task<List<T>> ToListAsync()
        {
            if (ShouldShift())
            {
                var queryable = AddShiftDynamicTypeAtTail(ReplacedQueryable);
                return ShiftList(await queryable.ToListAsync());
            }
            return await ReplacedQueryable.ToListAsync();
        }
        internal T[] ToArray()
        {
            if (ShouldShift())
            {
                var queryable = AddShiftDynamicTypeAtTail(ReplacedQueryable);
                return ShiftArray(queryable.ToList());
            }
            return ReplacedQueryable.ToArray();
        }

        internal async Task<T[]> ToArrayAsync()
        {
            if (ShouldShift())
            {
                var queryable = AddShiftDynamicTypeAtTail(ReplacedQueryable);
                return ShiftArray(await queryable.ToListAsync());
            }
            return await ReplacedQueryable.ToArrayAsync();
        }

        private List<Tuple<MemberExpression, Expression>> GetPropertyAccessorPairs()
        {
            var accessorPair = Visitor.IncludeExpressions
                                        .Select(_ => _.Arguments)
                                        .Select(args => CreatePair(args))
                                        .Where(pair => pair != null).ToList();

            return ExpressionHelper.DistinctPropertyAccessorPairs(accessorPair).ToList();
        }

        private Tuple<MemberExpression, Expression> CreatePair(ReadOnlyCollection<Expression> arguments)
        {
            var p = ((ConstantExpression)arguments[1]).Value;
            var s = ((ConstantExpression)arguments[2]).Value;

            if (p is string)
            {
                p = ExpressionHelper.PathToExpression(typeof (T), p.ToString());
            }

            var pBody = (p as LambdaExpression)?.Body ?? (Expression)p;
            if (!ExpressionHelper.CheckMemberAccess(typeof(T), pBody))
            {
                return null;
            }

            return Tuple.Create((MemberExpression)pBody, ((LambdaExpression)s).Body);
        }

        private bool ShouldShift()
        {
            return Visitor.QueryableTypeAtFirst == typeof (IQueryable<T>);
        }

        private IQueryable<object> AddShiftDynamicTypeAtTail(IQueryable<T> queryable)
        {
            var accessorPair = GetPropertyAccessorPairs();
            var shiftWith = ExpressionBuilder.ShiftWith<T>(DynamicType, accessorPair);

            return queryable.Select(shiftWith);
        } 

        private List<T> ShiftList(List<object> list)
        {
            var shift = DelegateProvider.Shift(DynamicType, typeof(T));
            return list.Select(_ => (T)shift(_)).ToList();
        }

        private T[] ShiftArray(List<object> list)
        {
            var shift = DelegateProvider.Shift(DynamicType, typeof(T));
            return list.Select(_ => (T)shift(_)).ToArray();
        }
    }
}
