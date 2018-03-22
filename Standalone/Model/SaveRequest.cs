using System;
using Standalone.Core;

namespace Standalone.Model
{
    public class SaveRequest<TKey, TValue>
    {
        public bool MakeSnapshot { get; set; }
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public SaveRequest<dynamic, dynamic> AsDynamic() =>
            new SaveRequest<dynamic, dynamic>()
            {
                MakeSnapshot = this.MakeSnapshot,
                Key = (dynamic)this.Key,
                Value = (dynamic)this.Value
            };

        public static explicit operator SaveRequest<dynamic, dynamic>(
            SaveRequest<TKey, TValue> src) =>
            src.AsDynamic();
    }

    public static class SaveRequest
    {
        public static Type For(DynamicDiffstore backend) =>
            typeof(SaveRequest<,>).MakeGenericType(backend.KeyType, backend.ValueType);
    }
}