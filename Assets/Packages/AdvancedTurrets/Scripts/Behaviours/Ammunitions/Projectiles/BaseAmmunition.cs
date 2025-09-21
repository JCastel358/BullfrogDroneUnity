using AdvancedTurrets.Behaviours.Collisions;
using AdvancedTurrets.Behaviours.Turrets;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Serialization;
using UnityEngine;
using UnityEngine.Events;

namespace AdvancedTurrets.Behaviours.Ammunitions
{
    /// <summary>
    /// Handles the kinematic behavior of ammunition including launching, trajectory tracking,
    /// and lifecycle events. Designed to be extensible and for custom ammunition behaviors to extend.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(StaticColliderGroup))]
    public class BaseAmmunition : MonoBehaviour, IKinematic
    {
        [SerializeField]
        [Tooltip("The Rigidbody of this ammunition used for physics interactions.")]
        private LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.OnSelf);
        public Rigidbody Rigidbody => _lazyRigidbody.Get(this);

        [SerializeField]
        [Tooltip("TargetingModule for this ammunition. Target parameters may be set after launch.")]
        private LazyComponent<TargetingModule> _lazyTargetingModule = new(ComponentAncestry.InParent);
        public TargetingModule TargetingModule => _lazyTargetingModule.Get(this);

        [SerializeField]
        [Tooltip("Own static colliders whose collisions may be toggled with other collider groups.")]
        private LazyComponent<StaticColliderGroup> _lazyStaticColliderGroup = new(ComponentAncestry.InParent);
        public StaticColliderGroup StaticColliderGroup => _lazyStaticColliderGroup.Get(this);

        [Tooltip("Constant acceleration applied each fixed update.")]
        public Vector3 Acceleration;

        [Tooltip("Trajectory that this was launched with.")]
        public Trajectory Trajectory;

        [Tooltip("Whether or not this ammunition should have its rotation updated to match its velocity each frame.")]
        public bool LookTowardsVelocity = true;

        private bool _hasTrajectoryExpired;

        public Vector3 Position => transform.position;

        public Vector3 Velocity => Rigidbody.GetVelocity();

        public virtual bool HasGravity => Rigidbody.useGravity;

        public virtual Trajectory GetTrajectory(bool instantiatedOrEnabled = false, float duration = default)
        {
            return new(Position, Velocity, HasGravity, Acceleration, duration, instantiatedOrEnabled, transform);
        }

        protected virtual void Update()
        {
            if (LookTowardsVelocity)
            {
                transform.rotation = Quaternion.LookRotation(Velocity, transform.up);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (Acceleration != default)
            {
                Rigidbody.AddForce(Acceleration, ForceMode.Acceleration);
            }

            if (!_hasTrajectoryExpired && Trajectory?.Duration != default && Trajectory.HasElapsed())
            {
                TrajectoryExpired.Invoke(Trajectory);
                _hasTrajectoryExpired = true;
            }
        }

        /// <summary>
        /// Launches the ammunition along the given trajectory by settings its kinematic properties to that of the <see cref="Trajectory"/> 
        /// </summary>
        public virtual void Launch(Trajectory trajectory)
        {
            trajectory.Transform = transform;
            Trajectory = trajectory;

            transform.position = trajectory.Position;
            Rigidbody.SetVelocity(trajectory.Velocity);
            Rigidbody.useGravity = trajectory.HasGravity;
            Acceleration = trajectory.Acceleration;
            Rigidbody.angularVelocity = default;

            TrajectoryLaunched.Invoke(trajectory);
            _hasTrajectoryExpired = false;
        }

        [Tooltip("Invoked whenever the ammunition is launched along a trajectory")]
        public UnityEvent<Trajectory> TrajectoryLaunched = new();

        [Tooltip("Invoked when the launched trajectorys duration has expired.")]
        public UnityEvent<Trajectory> TrajectoryExpired = new();
    }
}
