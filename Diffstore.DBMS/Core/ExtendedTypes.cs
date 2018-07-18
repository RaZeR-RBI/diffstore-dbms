using System;
using Diffstore.Entities;
using Diffstore.Snapshots;

/// <summary>
/// This namespace contains core classes needed for driver functionality.
/// </summary>
namespace Diffstore.DBMS.Core
{
    /// <summary>
    /// This class provides parameterless constructor for deserialization.
    /// </summary>
    public class EntityExt<TKey, TValue> : Entity<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        /// <summary>
        /// Used for deserialization by driver implementations.
        /// </summary>
        public EntityExt() : base(default(TKey), default(TValue)) { }

        /// <summary>
        /// Helper wrapper around existing function.
        /// </summary>
        public Entity<TKey, TValue> Create() => Entity.Create(Key, Value);
    }

    /// <summary>
    /// This class provides parameterless constructor for deserialization.
    /// </summary>
    public class SnapshotExt<TKey, TValue> : Snapshot<TKey, TValue>
        where TKey : IComparable
        where TValue : class, new()
    {
        /// <summary>
        /// Same as Snapshot.Time.
        /// </summary>
        public new long Time { get; set; }
        /// <summary>
        /// Same as Snapshot.State.
        /// </summary>
        /// <returns></returns>
        public new EntityExt<TKey, TValue> State { get; set; }

        /// <summary>
        /// Used for deserialization by driver implementations.
        /// </summary>
        public SnapshotExt() : base(default(long), default(Entity<TKey, TValue>)) { }

        /// <summary>
        /// Helper wrapper around existing function.
        /// </summary>
        public Snapshot<TKey, TValue> Create() =>
            Snapshot.Create(this.Time, this.State.Create());
    }
}