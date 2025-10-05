using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTurrets.Serialization
{
    /// <summary>
    /// Enables customization of component(s) by allowing serialization in the editor or lazy evaluation at runtime.
    /// </summary>
    [System.Serializable]
    public class LazyComponents<T> where T : Component
    {
        [SerializeField]
        [Tooltip("Determines how/where to get the components when resolved")]
        private ComponentAncestry _componentAncestry;

        [SerializeField]
        [Tooltip("The cached components. If resolved is set to True in the editor, this will be unmodified as if it were serialized.")]
        private T[] _value;

        [SerializeField]
        [Tooltip("Indicates whether or not the lazy has been resolved.")]
        private bool _hasValue;

        public LazyComponents(ComponentAncestry componentsAncestry)
        {
            _componentAncestry = componentsAncestry;
        }

        /// <summary>
        /// Gets the cached components if available or lazily resolves it based on the specified source.
        /// </summary>
        public T[] Get(Component source)
        {
            if (!_hasValue)
            {
                var value = GetComponents(source);
#if UNITY_EDITOR  // Editor accessing this won't actually cache otherwise this would be unusable!!
                if (!Application.isPlaying)
                {
                    return value;
                }
#endif
                _value = value;
                _hasValue = true;
            }

            return _value;
        }

        /// <summary>
        /// Cleares the cached values and re-enables lazy evaluation.
        /// </summary>
        public void Reset()
        {
            _value = default;
            _hasValue = default;
        }

        private T[] GetComponents(Component source)
        {
            return _componentAncestry switch
            {
                ComponentAncestry.InParent => source.GetComponentsInParent<T>(),
                ComponentAncestry.OnSelf => source.GetComponents<T>(),
                ComponentAncestry.InChildren => source.GetComponentsInChildren<T>(),
                ComponentAncestry.InChildrenOrSiblingsChildren => GetComponentsInChildrenOrSiblings(source),
                ComponentAncestry.InWorld => Object.FindObjectsByType<T>(FindObjectsSortMode.None),
                _ => throw new System.NotImplementedException()
            };
        }

        private static T[] GetComponentsInChildrenOrSiblings(Component source)
        {
            var parent = source.transform.parent;
            if (!parent)
            {
                return source.GetComponentsInChildren<T>();
            }

            var results = new List<T>();
            var childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childTransform = parent.GetChild(i);
                results.AddRange(childTransform.GetComponentsInChildren<T>());
            }

            return results.ToArray();
        }
    }
}
