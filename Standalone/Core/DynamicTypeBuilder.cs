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
using FluentIL;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Standalone.Core
{
    public static class DynamicTypeBuilder
    {
        public static Type CreateFrom(SchemaDefinition schema)
        {
            var references = CollectReferences(
                typeof(object), typeof(Attribute), typeof(IDiffstore<,>)
            );
            var compilation = CSharpCompilation.Create($"{Guid.NewGuid()}")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(GenerateCodeFor(schema));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    throw new NotSupportedException("Could not compile schema class: " +
                        result.Diagnostics.Aggregate("", (cur, next) =>
                            cur += " " + next.GetMessage()
                        ));
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
            code.AppendLine("public class EntityType {");
            foreach (var field in schema.Fields)
            {
                if (field.IgnoreChanges) code.AppendLine($"[{ignoreChangesAttr}]");
                if (field.DoNotPersist) code.AppendLine($"[{doNotPersistAttr}]");
                code.AppendLine($"public {field.Type} {field.Name} " + " { get; set; }");
            }
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