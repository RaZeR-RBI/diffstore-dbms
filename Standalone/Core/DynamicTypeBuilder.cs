using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using Diffstore;
using Diffstore.Entities;
using Diffstore.Snapshots;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Standalone.Core
{
    public static class DynamicTypeBuilder
    {
        public static Type CreateFrom(SchemaDefinition schema)
        {
            Console.WriteLine("Creating data type");
            var references = CollectReferences(
                typeof(object), typeof(Attribute), typeof(IDiffstore<,>)
            );
            var code = GenerateCodeFor(schema);
            var compilation = CSharpCompilation.Create($"{Guid.NewGuid()}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(code);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    throw new NotSupportedException("Could not compile schema class: " +
                        result.Diagnostics.Aggregate("", (cur, next) =>
                            cur += " " + next.GetMessage()
                        ) + "\nSource code:\n" + code);
                }
                ms.Seek(0, SeekOrigin.Begin);
                var loadContext = AssemblyLoadContext.Default;
                var assembly = loadContext.LoadFromStream(ms);
                return assembly.GetType("EntityType");
            }
        }

        private static SyntaxTree GenerateCodeFor(SchemaDefinition schema)
        {
            var ignoreChangesAttr = $"{typeof(IgnoreChangesAttribute)}";
            var doNotPersistAttr = $"{typeof(DoNotPersistAttribute)}";
            var code = new StringBuilder();
            code.AppendLine("using System; using System.Collections.Generic;");
            code.AppendLine("public class EntityType {");

            foreach (var field in schema.Fields)
            {
                if (field.IgnoreChanges) code.AppendLine($"[{ignoreChangesAttr}]");
                if (field.DoNotPersist) code.AppendLine($"[{doNotPersistAttr}]");
                code.AppendLine($"public {field.Type} {field.Name} " + " { get; set; }");
            }

            code.AppendLine(@"
                public override bool Equals(object other)
                {
                    return Equals(other as EntityType);
                }
           ");

            code.AppendLine("private bool Equals(EntityType other) {");
            var condition = string.Join(" && ",
                schema.Fields.Select(f => $"{f.Name} == other.{f.Name}"));
            code.AppendLine($"return {condition};");
            code.AppendLine("}");

            code.AppendLine("public override int GetHashCode() { unchecked {");
            code.AppendLine("int hashCode = 0;");
            foreach (var field in schema.Fields)
            {
                var isPrimitive = TypeResolver.FromName(field.Type).IsPrimitive;
                var getter = isPrimitive ? 
                    $"(int){field.Name}" : 
                    $"({field.Name} != null ? {field.Name}.GetHashCode() : 0)";
                code.AppendLine($"hashCode = (hashCode*397)^{getter};");
            }
            code.AppendLine("return hashCode; } }");

            code.AppendLine("}");
            return CSharpSyntaxTree.ParseText($"{code}");
        }

        private static List<MetadataReference> CollectReferences(params Type[] types)
        {
            var assemblies = new HashSet<Assembly>();
            types.ToList()
                .ForEach(type => Collect(type.Assembly));

            return assemblies
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Cast<MetadataReference>()
                .ToList();

            void Collect(Assembly assembly)
            {
                if (!assemblies.Add(assembly))
                    return;

                assembly.GetReferencedAssemblies()
                    .ToList()
                    .ForEach(name => assemblies.Add(Assembly.Load(name)));
            }
        }
    }
}