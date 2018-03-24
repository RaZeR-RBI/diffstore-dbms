using System;
using Diffstore.Entities;
using Diffstore.Snapshots;

namespace Diffstore.DBMS.Core
{
    // These classes provide parameterless constructors for deserialization
    public class EntityExt<TKey, TValue> : Entity<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        public EntityExt() : base(default(TKey), default(TValue)) { }

        public Entity<TKey, TValue> Create() => Entity.Create(Key, Value);
    }

    public class SnapshotExt<TKey, TValue> : Snapshot<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        public new EntityExt<TKey, TValue> State { get; set; }

        public SnapshotExt() : base(default(long), default(Entity<TKey, TValue>)) { }

        public Snapshot<TKey, TValue> Create() =>
            Snapshot.Create(base.Time, this.State.Create());
    }
}