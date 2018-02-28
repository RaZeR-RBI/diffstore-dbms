using System;
using System.Collections.Generic;
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
            var properties = schema.Fields.Select(field =>
                CreateProperty(type,
                    field.Name,
                    TypeResolver.FromName(field.Type),
                    field.IgnoreChanges,
                    field.DoNotPersist)
                    );

            // TODO GetHashCode
            CreateEquals(type, properties);
            return type.CreateType();
        }

        private static (MethodBuilder getter, MethodBuilder setter) CreateProperty(
            TypeBuilder tb, string propertyName,
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
            return (getPropMthdBldr, setPropMthdBldr);
        }

        private static void AddAttribute<T>(this PropertyBuilder builder)
        {
            var type = typeof(T);
            var ctorInfo = type.GetConstructor(new Type[] { });
            var attr = new CustomAttributeBuilder(ctorInfo, new object[] { });
            builder.SetCustomAttribute(attr);
        }

        private static void CreateEquals(TypeBuilder type,
            IEnumerable<(MethodBuilder getter, MethodBuilder setter)> properties)
        {
            var getMethods = properties
                .Select(m => m.getter)
                .ToList();

            // private bool Equals(DynamicType other)
            var equalsImpl = type.DefineMethod("Equals",
                MethodAttributes.Private |
                MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(bool),
                Type.EmptyTypes);
            equalsImpl.SetParameters(equalsImpl.DeclaringType);

            var impl = equalsImpl.GetILGenerator();
            getMethods.ForEach(m =>
            {
                impl.Emit(OpCodes.Ldarg_0); // this
                impl.EmitCall(OpCodes.Call, m, Type.EmptyTypes);
                impl.Emit(OpCodes.Ldarg_1); // DynamicType other
                impl.EmitCall(OpCodes.Call, m, Type.EmptyTypes);
                if (m.ReturnType.IsPrimitive)
                    impl.Emit(OpCodes.Ceq);
                else
                {
                    var equals = m.ReturnType.GetMethod("Equals", new [] { typeof(object) });
                    impl.EmitCall(OpCodes.Call, equals, Type.EmptyTypes);
                }
            });

            for (int i = 0; i < getMethods.Count - 1; i++)
                impl.Emit(OpCodes.And);

            impl.Emit(OpCodes.Ret);

            // public override bool Equals(object other)
            var equalsMethod = type.DefineMethod("Equals",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(bool),
                new[] { typeof(object) });

            var equalsIl = equalsMethod.GetILGenerator();
            equalsIl.Emit(OpCodes.Ldarg_0); // this
            equalsIl.Emit(OpCodes.Ldarg_1); // object other
            equalsIl.Emit(OpCodes.Isinst, equalsMethod.DeclaringType);
            // return Equals(other as DynamicType)
            equalsIl.EmitCall(OpCodes.Call, equalsImpl.GetBaseDefinition(), Type.EmptyTypes);
            equalsIl.Emit(OpCodes.Ret);
        }
    }
}