using AdvancedTurrets.Kinematics;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// Generates a trajectory from its configured target parameters.
    /// </summary>
    public class TargetingModule : MonoBehaviour
    {
        public enum TargetType
        {
            Default,
            Custom,
            Component,
            Rigidbody,
            IKinematic
        }

        [Tooltip("Defines how the target's trajectory is created. This will influence where the position, velocity, gravity, and acceleration values are sourced.")]
        public TargetType Type;

        [Tooltip("If the target type is IKinematic, this IKinematic's trajectory function will be used.")]
        public GameObject IKinematicGameObject;

        [Tooltip("If the target type is Rigidbody, this Rigidbody will be used to create the trajectory.")]
        public Rigidbody Rigidbody;

        [Tooltip("If the target type is Component, this Component will be used to create the trajectory")]
        public Component Component;

        [Tooltip("If the target type is Custom, this position will be used when creating the trajectory.")]
        public Vector3 Position;

        [Tooltip("If the target type is Custom, this velocity will be used when creating the trajectory.")]
        public Vector3 Velocity;

        [Tooltip("If the target type is Custom, this gravitry toggle will be used when creating the trajectory.")]
        public bool HasGravity;

        [Tooltip("If the target type is Custom, this acceleration will be used when creating the trajectory.")]
        public Vector3 Acceleration;

        [Tooltip("If true, disabled gameobjects will be considered invalid targets (as if they were null).")]
        public bool InactiveTargetsInvalid = true;

        /// <summary>
        /// Retrieves the trajectory from the given targeting parameters.
        /// </summary>
        public bool GetTargetTrajectory(out Trajectory trajectory, bool targetInstantiatedOrEnabled = false, float duration = default)
        {
            if (HasTarget())
            {
                trajectory = Type switch
                {
                    TargetType.Custom => new(Position, Velocity, HasGravity, Acceleration, duration, targetInstantiatedOrEnabled),
                    TargetType.Component => new(Component, default, default, default, duration, targetInstantiatedOrEnabled),
                    TargetType.Rigidbody => new(Rigidbody, default, duration, targetInstantiatedOrEnabled),
                    TargetType.IKinematic => GetIKinematicTrajectory(targetInstantiatedOrEnabled, duration),
                    _ => default,
                };
                return true;
            }

            trajectory = default;
            return false;
        }

        private Trajectory GetIKinematicTrajectory(bool instantiatedOrEnabled, float duration)
        {
            if (IKinematicGameObject && IKinematicGameObject.TryGetComponent<IKinematic>(out var iKinematic))
            {
                return iKinematic.GetTrajectory(instantiatedOrEnabled, duration);
            }

            Debug.LogError($"Unable to find {nameof(IKinematic)} on GameObject {IKinematicGameObject}");
            return default;
        }

        /// <summary>
        /// Determines whether this has a valid target based on its targeting parameters..
        /// </summary>
        public bool HasTarget()
        {
            return Type switch
            {
                TargetType.Custom => true,
                TargetType.Component => Component && (InactiveTargetsInvalid || Component.gameObject.activeInHierarchy),
                TargetType.Rigidbody => Rigidbody && (InactiveTargetsInvalid || Rigidbody.gameObject.activeInHierarchy),
                TargetType.IKinematic => IKinematicGameObject && (InactiveTargetsInvalid || IKinematicGameObject.activeInHierarchy),
                _ => false,
            };
        }

        /// <summary>
        /// Attempts to get the given component on the target (if possible)
        /// </summary>
        public T GetComponentOnTarget<T>() where T : Component
        {
            if (GetTargetGameObject() is GameObject gameObject)
            {
                return gameObject.GetComponent<T>();
            }

            return default;
        }

        public GameObject GetTargetGameObject()
        {
            return Type switch
            {
                TargetType.Component => Component ? Component.gameObject : default,
                TargetType.Rigidbody => Rigidbody ? Rigidbody.gameObject : default,
                TargetType.IKinematic => IKinematicGameObject ? IKinematicGameObject : default,
                _ => default
            };
        }

        public void SetTarget<T>(T tComponent) where T : Component, IKinematic
        {
            Type = TargetType.IKinematic;
            IKinematicGameObject = tComponent?.gameObject;
        }

        public void SetTarget(Rigidbody rigidbody)
        {
            Type = TargetType.Rigidbody;
            Rigidbody = rigidbody;
        }

        public void SetTarget(Component component)
        {
            Type = TargetType.Component;
            Component = component;
        }

        public void SetTarget(Vector3 position, Vector3 velocity = default, Vector3 acceleration = default, bool hasGravity = false)
        {
            Type = TargetType.Custom;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            HasGravity = hasGravity;
        }

        /// <summary>
        /// Copies the current targeting parameters to another TargetingModule.
        /// </summary>
        public void CopyTo(TargetingModule targetingModule)
        {
            if (targetingModule == this || targetingModule == null)
            {
                return;
            }

            targetingModule.Type = Type;
            targetingModule.Position = Position;
            targetingModule.Velocity = Velocity;
            targetingModule.HasGravity = HasGravity;
            targetingModule.Acceleration = Acceleration;
            targetingModule.Component = Component;
            targetingModule.Rigidbody = Rigidbody;
            targetingModule.IKinematicGameObject = IKinematicGameObject;
        }

        /// <summary>
        /// Updates parameters so no target is defined.
        /// </summary>
        public void ClearTarget()
        {
            Type = TargetType.Default;
        }
    }
}
