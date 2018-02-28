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
        private struct PropertyEmitInfo
        {
            public MethodBuilder Getter;
            public MethodBuilder Setter;
            public FieldBuilder BackingField;
        }

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

            CreateEquals(type, properties);
            CreateGetHashCode(type, properties);
            return type.CreateType();
        }

        private static PropertyEmitInfo CreateProperty(
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
            return new PropertyEmitInfo()
            {
                Getter = getPropMthdBldr,
                Setter = setPropMthdBldr,
                BackingField = fieldBuilder
            };
        }

        private static void AddAttribute<T>(this PropertyBuilder builder)
        {
            var type = typeof(T);
            var ctorInfo = type.GetConstructor(new Type[] { });
            var attr = new CustomAttributeBuilder(ctorInfo, new object[] { });
            builder.SetCustomAttribute(attr);
        }

        private static void CreateEquals(TypeBuilder type,
            IEnumerable<PropertyEmitInfo> properties)
        {
            var getMethods = properties
                .Select(m => m.Getter)
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
                    var equals = typeof(Object).GetMethod("Equals", new[] {
                        typeof(Object), typeof(Object)
                    });
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
            equalsIl.EmitCall(OpCodes.Call, equalsImpl, Type.EmptyTypes);
            equalsIl.Emit(OpCodes.Ret);
        }

        private static void CreateGetHashCode(TypeBuilder type,
            IEnumerable<PropertyEmitInfo> properties)
        {
            var fields = properties
                .Select(p => p.BackingField)
                .ToList();

            var refHashHelper = CreateRefHashHelper(type);

            // public override int GetHashCode() 
            var getHashCodeImpl = type.DefineMethod("GetHashCode",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(int),
                Type.EmptyTypes);

            var il = getHashCodeImpl.GetILGenerator();
            il.DeclareLocal(typeof(long));
            il.Emit(OpCodes.Ldc_I8, 0L);
            il.Emit(OpCodes.Stloc_0);

            fields.ForEach(f =>
            {
                // hash = (hash * 397) ^ value
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, f);
                if (!f.FieldType.IsPrimitive)
                    il.EmitCall(OpCodes.Call, refHashHelper, Type.EmptyTypes);
                il.Emit(OpCodes.Conv_I8); // expand to int64
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I8, 397L);
                il.Emit(OpCodes.Mul);
                il.Emit(OpCodes.Xor);
                il.Emit(OpCodes.Stloc_0);
            });
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Conv_I4); // cast back to int32
            il.Emit(OpCodes.Ret);
        }

        private static MethodBuilder CreateRefHashHelper(TypeBuilder type)
        {
            // private int RefHash(object obj)
            var refHashHelper = type.DefineMethod("RefHash",
                MethodAttributes.Private |
                MethodAttributes.Static |
                MethodAttributes.HideBySig,
                CallingConventions.Standard,
                typeof(int),
                new[] { typeof(object) });

            var il = refHashHelper.GetILGenerator();
            var notNullLabel = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Brtrue_S, notNullLabel);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(notNullLabel);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, 
                typeof(object).GetMethod("GetHashCode"),
                Type.EmptyTypes);
            il.Emit(OpCodes.Ret);
            return refHashHelper;
        }
    }
}