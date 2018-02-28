using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Diffstore.Entities;
using Diffstore.Snapshots;
using FluentIL;

namespace Standalone.Core
{
    public static class DynamicTypeBuilder
    {
        public static Type CreateFrom(SchemaDefinition schema)
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName($"{Guid.NewGuid()}"),
                AssemblyBuilderAccess.Run
            );
            var module = assembly.DefineDynamicModule("Default");
            var type = module.DefineType("DynamicType",
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
            type.DefineDefaultConstructor(MethodAttributes.Public);
            foreach (var field in schema.Fields)
                CreateProperty(type,
                    field.Name,
                    TypeResolver.FromName(field.Type),
                    field.IgnoreChanges,
                    field.DoNotPersist);

            CreateEquals(type);
            return type.CreateType();
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName,
            Type propertyType, bool ignoreChanges = false, bool doNotPersist = false)
        {
            var fieldBuilder =
                tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder =
                tb.DefineProperty(propertyName, PropertyAttributes.HasDefault,
                propertyType, null);

            if (ignoreChanges) propertyBuilder.AddAttribute<IgnoreChangesAttribute>();
            if (doNotPersist) propertyBuilder.AddAttribute<DoNotPersistAttribute>();

            var getPropMthdBldr =
                tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                propertyType, Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        private static void AddAttribute<T>(this PropertyBuilder builder)
        {
            var type = typeof(T);
            var ctorInfo = type.GetConstructor(new Type[] { });
            var attr = new CustomAttributeBuilder(ctorInfo, new object[] { });
            builder.SetCustomAttribute(attr);
        }

        private static void CreateEquals(TypeBuilder type)
        {
            var method = type.DefineMethod("Equals", 
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig, 
                CallingConventions.HasThis,
                typeof(bool),
                new [] { typeof(object) });

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldc_I4_1); // return true (for now)
            il.Emit(OpCodes.Ret);
            // TODO
        }
    }
}