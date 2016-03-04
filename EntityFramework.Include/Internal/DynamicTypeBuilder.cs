using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace EntityFramework.Include.Internal
{
    internal class DynamicTypeBuilder
    {
        private static ModuleBuilder ModuleBuilder { get; }

        static DynamicTypeBuilder()
        {
            var assemblyName = new AssemblyName("EntityFramework.Include.Dynamic");
            ModuleBuilder =
                Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                    .DefineDynamicModule(assemblyName.Name, false);
        }

        internal static Type Build(Type target)
        {
            var typeBuilder = ModuleBuilder.DefineType(GetDynamicTypeName(target),
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                target);

            var createdType = typeBuilder.CreateType();
            return createdType;
        }

        private static string GetDynamicTypeName(Type target)
        {
            return target.Name + "_Dynamic";
        }
    }
}
