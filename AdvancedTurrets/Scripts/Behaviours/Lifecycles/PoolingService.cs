using AdvancedTurrets.Libraries;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Singleton service that manages object pooling for GameObjects and Components.
    /// Handles the creation, retrieval, and destruction of pooled instances.
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue - 1)]
    public class PoolingService : SingletonBehaviour<PoolingService>
    {
        protected override bool DestroyOldInstanceOnDuplication => true;

        private readonly Dictionary<GameObject, Pool> _prefabToPool = new();
        private readonly Dictionary<GameObject, Pool> _instanceToPool = new();

        public void Clear()
        {
            _prefabToPool.ForEach(kvp => Destroy(kvp.Value.gameObject));
        }

        private void LateUpdate()
        {
            _prefabToPool.RemoveWhere(kvp => !kvp.Key || !kvp.Value);
            _instanceToPool.RemoveWhere(kvp => !kvp.Key || !kvp.Value);
        }

        public Pool GetOrCreatePoolFromPrefab(GameObject prefab)
        {
            if (!_prefabToPool.TryGetValue(prefab, out var pool) || pool == null)
            {
                pool = _prefabToPool[prefab] = new GameObject($"Pool {prefab.name}").AddComponent<Pool>();
                pool.transform.SetParent(transform);
                pool.Initialize(prefab);
            }

            return pool;
        }

        public Pool GetPoolFromInstance(GameObject instance)
        {
            return _instanceToPool.TryGetValue(instance, out var pool) ? pool : null;
        }

        # region GameObject Pooling

        /// <summary>
        /// Retrieves or creates a pooled instance of the specified prefab at a given position and rotation.
        /// </summary>
        public GameObject GetOrCreate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var pool = GetOrCreatePoolFromPrefab(prefab);
            var instance = pool.GetOrCreate(position, rotation);
            _instanceToPool[instance] = pool;
            return instance;
        }

        /// <summary>
        /// Retrieves or creates a pooled instance of the specified prefab with a given parent.
        /// </summary>
        public GameObject GetOrCreate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool = GetOrCreatePoolFromPrefab(prefab);
            var instance = pool.GetOrCreate(position, rotation, parent);
            _instanceToPool[instance] = pool;
            return instance;
        }

        /// <summary>
        /// Returns an instance to the pool or destroys it if no associated pool is found.
        /// </summary>
        public void PoolOrDestroy(GameObject instance)
        {
            if (GetPoolFromInstance(instance) is Pool pool)
            {
                pool.PoolInstance(instance);
            }
            else
            {
                Destroy(instance);
            }
        }

        #endregion

        # region Generic Component Pooling

        /// <summary>
        /// Retrieves or creates a pooled instance of a specified component type.
        /// </summary>
        public T GetOrCreate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return GetOrCreate(prefab.gameObject, position, rotation).GetComponent<T>();
        }

        /// <summary>
        /// Retrieves or creates a pooled instance of a specified component type with a given parent.
        /// </summary>
        public T GetOrCreate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Component
        {
            return GetOrCreate(prefab.gameObject, position, rotation, parent).GetComponent<T>();
        }

        /// <summary>
        /// Returns a component instance to the pool or destroys it if no associated pool is found.
        /// </summary>
        public void PoolOrDestroy<T>(T instance) where T : Behaviour
        {
            PoolOrDestroy(instance.gameObject);
        }

        #endregion

        /// <summary>
        /// Manages object pooling for a given source (instance or prefab).
        /// This class is controlled by <see cref="PoolingService"/> and maintains active and inactive instances.
        /// </summary>
        [AddComponentMenu("")]
        [DefaultExecutionOrder(int.MaxValue)]
        public class Pool : MonoBehaviour
        {
            [Tooltip("Tracks how often the pool successfully reuses an existing instance instead of instantiating a new one.")]
            public int Hits;

            [Tooltip("Tracks how often the pool needed to instantiate a new instance instead of reusing an existing one.")]
            public int Misses;

            private HashSet<GameObject> Active { get; } = new();

            private HashSet<GameObject> Inactive { get; } = new();

            public int ActiveCount => Active.Count;

            public int InactiveCount => Inactive.Count;

            public int TotalCount => ActiveCount + InactiveCount;

            private GameObject Source { get; set; }

            private string _lastPrefabName;

            private readonly HashSet<GameObject> _framePurgatory = new();

            private void Update()
            {
                if (Source == null)
                {
                    transform.name = $"<Null> ({_lastPrefabName}) R: {Active.Count}";
                    Inactive.ForEach(go => Destroy(go));

                    if (Active.Count == 0)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    _lastPrefabName = Source.name;
                    transform.name = $"{_lastPrefabName} ({Misses}/{Hits + Misses})";
                }

                Active.RemoveWhere(go => go == null);
                Inactive.RemoveWhere(go => go == null);
                _framePurgatory.Clear();
            }

            public void Initialize(GameObject source)
            {
                Source = source;
            }

            public GameObject GetOrCreate(Vector3 position, Quaternion rotation)
            {
                return GetOrCreate(position, rotation, transform);
            }

            public GameObject GetOrCreate(Vector3 position, Quaternion rotation, Transform parent)
            {
                if (GetNextInactive(position, rotation, parent) is not GameObject instance)
                {
                    instance = Instantiate(Source, position, rotation, parent);
                    instance.name = $"{Source.name} ({Misses})";
                    Misses++;
                }

                Active.Add(instance);
                return instance;
            }

            private GameObject GetNextInactive(Vector3 position, Quaternion rotation, Transform parent)
            {
                var nextInactive = Inactive.FirstOrDefault(go => go && !_framePurgatory.Contains(go));

                if (nextInactive != null)
                {
                    Inactive.Remove(nextInactive);

                    nextInactive.transform.SetParent(parent);
                    nextInactive.transform.SetPositionAndRotation(position, rotation);
                    nextInactive.SetActive(true);

                    Hits++;
                    return nextInactive;
                }

                return null;
            }

            public void PoolInstance(GameObject instance)
            {
                if (instance == null)
                {
                    return;
                }

                if (Active.Contains(instance))
                {
                    Active.Remove(instance);
                    Inactive.Add(instance);

                    instance.transform.SetParent(transform);
                    instance.SetActive(false);

                    _framePurgatory.Add(instance);
                }
                else
                {
                    // Prevent duplicate pooling calls if multiple sources try to pool the same object
                    if (Inactive.Contains(instance))
                    {
                        return;
                    }

                    Debug.LogWarning($"Instance {instance.name} no longer matches the pool source. Destroying.");
                    Destroy(instance);
                }
            }
        }
    }
}
