using System.Collections.Generic;

namespace Standalone.Core
{
    public class SchemaDefinition
    {
        public IList<FieldDefinition> Fields { get; set; }
    }

    public class FieldDefinition
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IgnoreChanges { get; set; }
        public bool DoNotPersist { get; set; }
    }

    public static class SchemaDefinitionExtensions
    {
        public static SchemaDefinition WithField(this SchemaDefinition schema,
            string name, string type,
            bool ignoreChanges = false, bool doNotPersist = false)
            {
                if (schema.Fields == null)
                    schema.Fields = new List<FieldDefinition>();
                schema.Fields.Add(new FieldDefinition()
                {
                    Name = name,
                    Type = type,
                    IgnoreChanges = ignoreChanges,
                    DoNotPersist = doNotPersist,
                });
                return schema;
            }
    }
}