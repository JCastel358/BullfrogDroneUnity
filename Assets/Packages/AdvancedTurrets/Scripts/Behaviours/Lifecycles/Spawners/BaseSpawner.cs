using System.Collections.Generic;
using System.Linq;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Kinematics;
using UnityEngine;
using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Behaviours.Effects;
using AdvancedTurrets.Behaviours.Lifecycles;


namespace AdvancedTurrets.Behaviours.Spawners
{
    /// <summary>
    /// Base class containing common spawning functions for managing object instantiation and pooling.
    /// Supports spawning prefabs at various locations, with options for following targets.
    /// All endpoints here are UnityEvent/Inspector friendly.
    /// </summary>
    public abstract class BaseSpawner<T> : MonoBehaviour where T : Component
    {
        public enum SpawnerTypes
        {
            All,
            Random,
        }

        [Tooltip("Specifies whether to spawn all prefabs or only a random one.")]
        public SpawnerTypes Type = SpawnerTypes.All;

        [Tooltip("If true, spawning is ignored when the component is disabled.")]
        public bool IgnoreSpawnsWhenDisabled = true;

        [Tooltip("If true, all spawned objects will be spawned through the PoolingService or not and can be recycled at the end of its lifetime.")]
        public bool Pool = true;

        [Tooltip("The prefabs from which this will select")]
        public T[] Prefabs;

        public void SpawnAtCollision(Collision collision)
        {
            var contact = collision.contacts[0];
            Spawn(contact.point, Quaternion.LookRotation(contact.normal), null);
        }

        public void SpawnAtTransform(Transform transform)
        {
            Spawn(transform.position, transform.rotation, null);
        }

        public void SpawnAtSelf()
        {
            SpawnAtTransform(transform);
        }

        public void SpawnAsChild()
        {
            SpawnAsChildOf(transform);
        }

        public void SpawnAsChildOf(Transform parent)
        {
            Spawn(parent.position, parent.rotation, parent);
        }

        public void SpawnAndFollow(Transform parent)
        {
            foreach (var instance in Spawn(parent.position, parent.rotation, null))
            {
                TrackingService.Instance.Follow(instance.transform, parent);
            }
        }

        public void SpawnAndFollow(Muzzle muzzle)
        {
            SpawnAndFollow(muzzle.transform);
        }

        public void SpawnAndFollow(Trajectory trajectory)
        {
            if (trajectory.Transform == null)
            {
                Debug.LogWarning("No transform attached to trajectory");
                return;
            }

            SpawnAndFollow(trajectory.Transform);
        }

        public void SpawnAtPosition(Vector3 position)
        {
            Spawn(position, Quaternion.identity, null);
        }

        public void SpawnAtPositionWithRotation(Vector3 position, Quaternion rotation)
        {
            Spawn(position, rotation, null);
        }

        public void SpawnAtRaycastHit(RaycastHit raycastHit)
        {
            Spawn(raycastHit.point, Quaternion.LookRotation(raycastHit.normal), null);
        }

        /// <summary>
        /// Spawns one or more prefabs at the specified position and rotation.
        /// </summary>
        private IEnumerable<T> Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (IgnoreSpawnsWhenDisabled && (!isActiveAndEnabled || !enabled))
            {
                return new T[0];
            }

            if (!gameObject.scene.isLoaded)
            {
                return new T[0];
            }

            return Type == SpawnerTypes.Random
                ? new[] { Spawn(Prefabs.RandomOrDefault(), position, rotation, parent) }
                : Prefabs.Select(p => Spawn(p, position, rotation, parent)).ToList();
        }

        /// <summary>
        /// Spawns one or more prefabs at the specified position and rotation and under the parent.
        /// </summary>
        private T Spawn(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (prefab == null) return null;

            T result = parent
                ? (Pool ? PoolingService.Instance.GetOrCreate(prefab, position, rotation, parent) : Instantiate(prefab, position, rotation, parent))
                : (Pool ? PoolingService.Instance.GetOrCreate(prefab, position, rotation) : Instantiate(prefab, position, rotation));

            OnSpawned(result);
            return result;
        }

        /// <summary>
        /// Called whenever a new object is spawned.
        /// Can be overridden in subclasses to implement custom behavior.
        /// </summary>
        protected virtual void OnSpawned(T spawnedObject) { }
    }
}
