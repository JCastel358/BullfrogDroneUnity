using UnityEngine;

namespace AdvancedTurrets.Serialization
{
    /// <summary>
    /// The supported ancestral navigations for resolving components in Unity.
    /// </summary>
    public enum ComponentAncestry
    {
        InParent,
        OnSelf,
        InChildren,
        InChildrenOrSiblingsChildren,
        InWorld
    }

    /// <summary>
    /// Enables customization of component(s) by allowing serialization in the editor or lazy evaluation at runtime.
    /// </summary>
    [System.Serializable]
    public class LazyComponent<T> where T : Component
    {
        [SerializeField]
        [Tooltip("Determines how/where to get the component when resolved")]
        private ComponentAncestry _componentAncestry;

        [SerializeField]
        [Tooltip("The cached component. If resolved is set to True in the editor, this will be unmodified as if it were serialized.")]
        private T _value;

        [SerializeField]
        [Tooltip("Indicates whether or not the lazy has been resolved.")]
        private bool _hasValue;

        public LazyComponent(ComponentAncestry componentAncestry)
        {
            _componentAncestry = componentAncestry;
        }

        /// <summary>
        /// Gets the cached component if available or lazily resolves it based on the specified source.
        /// </summary>
        public T Get(Component source)
        {
            if (!_hasValue)
            {
                var value = GetComponent(source);
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

        private T GetComponent(Component source)
        {
            return _componentAncestry switch
            {
                ComponentAncestry.InParent => source.GetComponentInParent<T>(),
                ComponentAncestry.OnSelf => source.GetComponent<T>(),
                ComponentAncestry.InChildren => source.GetComponentInChildren<T>(),
                ComponentAncestry.InChildrenOrSiblingsChildren => GetComponentInChildrenOrSiblings(source),
                ComponentAncestry.InWorld => Object.FindAnyObjectByType<T>(),
                _ => throw new System.NotImplementedException()
            };
        }

        private static T GetComponentInChildrenOrSiblings(Component source)
        {
            var parent = source.transform.parent;
            if (!parent)
            {
                return source.GetComponentInChildren<T>();
            }

            var result = source.GetComponentInChildren<T>();
            if (result)
            {
                return result;
            }

            var childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var childTransform = parent.GetChild(i);
                if (childTransform != source.transform)
                {
                    result = childTransform.GetComponentInChildren<T>();
                    if (result)
                    {
                        return result;
                    }
                }
            }

            return default;
        }
    }
}
