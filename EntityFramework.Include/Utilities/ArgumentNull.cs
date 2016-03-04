using System;

namespace EntityFramework.Include.Utilities
{
    internal class ArgumentNull
    {
        internal static void Check(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }
    }
}
