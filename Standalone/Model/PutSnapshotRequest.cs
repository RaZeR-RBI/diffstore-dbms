using System;
using Standalone.Core;

namespace Standalone.Model
{
    public class PutSnapshotRequest<TKey, TValue>
    {
        public long Time { get; set; }
        public EntityWrapper<TKey, TValue> State { get; set; }

        public PutSnapshotRequest<dynamic, dynamic> AsDynamic() =>
            new PutSnapshotRequest<dynamic, dynamic>()
            {
                Time = this.Time,
                State = new PutSnapshotRequest<dynamic, dynamic>.EntityWrapper<dynamic, dynamic> {
                    Key = (dynamic)this.State.Key,
                    Value = (dynamic)this.State.Value
                }
            };

        public class EntityWrapper<TK, TV>
        {   
            public TK Key { get; set; }
            public TV Value { get; set; }   

            public Diffstore.Entities.Entity<TK, TV> AsEntity() =>
                Diffstore.Entities.Entity.Create(Key, Value);
        }
    }

    public static class PutSnapshotRequest
    {
        public static Type For(DynamicDiffstore backend) =>
            typeof(PutSnapshotRequest<,>).MakeGenericType(backend.KeyType, backend.ValueType);
    }
}