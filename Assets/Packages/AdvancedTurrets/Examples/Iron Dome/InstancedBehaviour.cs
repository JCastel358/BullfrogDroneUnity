using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedTurrets.Behaviours
{
    /// <summary>
    /// Provides a cached static reference to all enabled instances of derived types.
    /// This avoids expensive `FindComponentsByType` calls every frame by maintaining a registry
    /// of active instances.
    /// </summary>
    public class InstancedBehaviour<T> : MonoBehaviour where T : InstancedBehaviour<T>
    {
        public static readonly HashSet<T> EnabledInstances = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ClearStaticResources()
        {
            EnabledInstances.Clear();
        }

        protected virtual void OnEnable()
        {
            var tThis = (T)this;
            EnabledInstances.Add(tThis);
            InstanceEnabled?.Invoke(tThis);
        }

        protected virtual void OnDisable()
        {
            var tThis = (T)this;
            EnabledInstances.Remove(tThis);
            InstanceDisabled?.Invoke(tThis);
        }

        public static event Action<T> InstanceEnabled;

        public static event Action<T> InstanceDisabled;
    }
}
