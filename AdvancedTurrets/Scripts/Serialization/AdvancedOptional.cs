using System;
using UnityEngine;

namespace AdvancedTurrets.Serialization
{
    /// <summary>
    /// A serializable container which can indicate whether or not it has a value.
    /// </summary>
    [Serializable]
    public class AdvancedOptional<T>
    {
        [SerializeField]
        [Tooltip("Indicates whether or not the value has been set.")]
        private bool _hasValue;
        public bool HasValue => _hasValue;

        [SerializeField]
        [Tooltip("The underlying value of T.")]
        private T _value;

        public T Value
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }
                else
                {
                    throw new Exception("Cannot retrieve value when HasValue is false!");
                }
            }
        }

        public AdvancedOptional() { }

        public AdvancedOptional(T value, bool hasValue = true)
        {
            Set(value, hasValue);
        }

        public void Set(T value, bool hasValue = true)
        {
            _value = value;
            _hasValue = hasValue;
        }

        /// <summary>
        /// Clears the value and resets to not having a value.
        /// </summary>
        public void Clear()
        {
            Set(default, false);
        }

        /// <summary>
        /// Sets the value only if HasValue is false.
        /// </summary>
        public void SetIfEmpty(T value)
        {
            if (!HasValue)
            {
                Set(value);
            }
        }

        /// <summary>
        /// Gets the value if it exists, otherwise returns the default value of T.
        /// </summary>
        public T GetValueOrDefault()
        {
            if (_hasValue)
            {
                return _value;
            }

            return default;
        }

        /// <summary>
        /// Gets the value if it exists, otherwise invokes the provided function to get a value.
        /// </summary>
        public T GetValueOrElse(Func<T> getValue)
        {
            if (_hasValue)
            {
                return _value;
            }

            return getValue.Invoke();
        }

        /// <summary>
        /// Attempts to get the value. Returns true if the value exists, otherwise false.
        /// </summary>
        public bool TryGet(out T value)
        {
            if (_hasValue)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        public static implicit operator T(AdvancedOptional<T> optional)
        {
            return optional.GetValueOrDefault();
        }

        public static implicit operator AdvancedOptional<T>(T t)
        {
            return new AdvancedOptional<T>(t);
        }
    }
}
