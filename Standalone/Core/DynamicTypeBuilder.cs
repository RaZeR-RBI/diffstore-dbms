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
            // TODO IgnoreChanges and DoNotSave attributes 
            foreach (var field in schema.Fields)
                CreateProperty(type, field.Name, PropertyTypeResolver.FromName(field.Type));

            return type.CreateType();
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = 
                tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = 
                tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, 
                propertyType, null);

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
    }
}