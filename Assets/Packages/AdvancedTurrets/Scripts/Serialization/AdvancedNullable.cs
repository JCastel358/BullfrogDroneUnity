namespace AdvancedTurrets.Serialization
{
    /// <summary>
    /// Extends <see cref="AdvancedOptional{T}"/> and adds a layer of implicit conversions to / from C# nullables.
    /// </summary>
    [System.Serializable]
    public class AdvancedNullable<T> : AdvancedOptional<T> where T : struct
    {
        public AdvancedNullable() : base() { }

        public AdvancedNullable(T value, bool hasValue = true) : base(value, hasValue) { }

        public static implicit operator AdvancedNullable<T>(T? t)
        {
            return t.HasValue ? new AdvancedNullable<T>(t.Value) : new AdvancedNullable<T>();
        }

        public static implicit operator AdvancedNullable<T>(T t)
        {
            return new AdvancedNullable<T>(t);
        }

        public static implicit operator T?(AdvancedNullable<T> tNullable)
        {
            return tNullable.TryGet(out var t) ? t : default;
        }
    }
}
