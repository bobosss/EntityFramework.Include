using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Include.Internal
{
    internal class DbContextHelper
    {
        internal static DbContext GetDbContext(IQueryProvider provider)
        {
            var internalContext =
                provider.GetType().GetProperty("InternalContext");
            var owner = internalContext?.PropertyType.GetProperty("Owner");

            return (DbContext)owner?.GetValue(internalContext.GetValue(provider));
        }
    }
}
