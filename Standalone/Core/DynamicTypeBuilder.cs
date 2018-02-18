using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using FluentIL;

namespace Standalone.Core
{
    public static class DynamicTypeBuilder
    {
        public static Type CreateFrom(SchemaDefinition schema)
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName($"{new Guid()}"),
                AssemblyBuilderAccess.Run
            );
            var module = assembly.DefineDynamicModule("Default");
            var type = module.DefineType("DynamicType");

            // TODO
            throw new NotImplementedException();
        }
    }
}