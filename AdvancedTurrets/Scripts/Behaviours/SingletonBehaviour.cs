using UnityEngine;

namespace AdvancedTurrets.Behaviours
{
    /// <summary>
    /// Traditional singleton instance - only one instance can exist.
    /// If a new instance is created while another exists, the new one will destroy the old instance.
    /// This behaviour allows you to replace or cleare caches of singleton objects by instantiating a new one.
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        protected abstract bool DestroyOldInstanceOnDuplication { get; }

        private static T _instance;

        /// <summary>
        /// If no instance already exists, a new one will be created.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance)
            {
                if (DestroyOldInstanceOnDuplication)
                {
                    // Destroy previous instance - make primary this new one
                    Destroy(_instance.gameObject);
                }
                else
                {
                    // Destroy this (new) instance - keep old one
                    Destroy(gameObject);
                    return;
                }
            }

            _instance = (T)this;
        }
    }
}
