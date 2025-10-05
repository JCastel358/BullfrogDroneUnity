using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Utilities;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Visualization;
using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Behaviours.Lifecycles;
using AdvancedTurrets.Serialization;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// Extends <see cref="BaseTurret"/> by adding kinematic-based properties and interceptions to launch ammunitions with.
    /// </summary>
    public class KinematicTurret : BaseTurret
    {
        [SerializeField]
        [Tooltip("The Rigidbody used for calculating inherited velocity for launched projectiles.")]
        private LazyComponent<Rigidbody> _lazyRigidbody = new(ComponentAncestry.InParent);
        public Rigidbody Rigidbody => _lazyRigidbody.Get(this);

        [Tooltip("Determines how the turret selectes trajectories. Mortar prefers longer flight-time trajectories.")]
        public bool Mortar;

        [Tooltip("Prefab for the ammunition to be instantiated by the turret when fired.")]
        public BaseAmmunition AmmunitionPrefab;

        [Min(0)]
        [Tooltip("The velocity (magnitude) at which projectiles will be launched.")]
        public float LaunchVelocity = 150f;

        [Range(0, 180)]
        [Tooltip("The random deviation (spread) in degrees from the intended firing direction.")]
        public float Spread;

        [Tooltip("Overrides the `HasGravity` property of fired projectiles if specified.")]
        public AdvancedNullable<bool> OverrideHasGravity = new();

        [Tooltip("Overrides the `Acceleration` property of fired projectiles if specified.")]
        public AdvancedNullable<Vector3> OverrideAcceleration = new();

        public enum VelocityTypes
        {
            Rigidbody,
            Custom,
            None
        }

        [Tooltip("Determines how the launched projectile's inherited velocity will be calculated.")]
        public VelocityTypes VelocityType = VelocityTypes.Rigidbody;

        [Tooltip("If VelocityType is custom, this velocity will be inherited by launched projectiles.")]
        public Vector3 CustomVelocity = default;

        public enum InterceptFailBehaviourTypes
        {
            None,
            AimStraightAtTarget,
            TimeOut,
            ClearTarget
        }

        [Tooltip("Defines the turrets behavior when an interception calculation fails.")]
        public InterceptFailBehaviourTypes InterceptFailureBehaviour = InterceptFailBehaviourTypes.AimStraightAtTarget;

        [Tooltip("The last trajectory used for aiming by the turret.")]
        public Trajectory LastAimTrajectory;

        protected virtual void Update()
        {
            var deltaTime = AdvancedTime.SmartDeltaTime;

            if (!TargetingModule.GetTargetTrajectory(out var target))
            {
                TimeOut(deltaTime);
                return;
            }

            if (!MuzzleLoader.GetNextEndEffector(out var endEffector))
            {
                TimeOut(deltaTime);
                return;
            }

            if (!AmmunitionPrefab)
            {
                Debug.LogWarning($"No Ammunition specified on {name}!");
                return;
            }

            TimeoutClock.Reset();

            if (!Intercept(out LastAimTrajectory, target, endEffector.Position))
            {
                HandleInterceptFailure(endEffector, target, deltaTime);
                return;
            }

            var aimPosition = LastAimTrajectory.Position + LastAimTrajectory.LaunchVelocity;
            if (!endEffector.AimAtPosition(aimPosition, deltaTime))
            {
                return;
            }

            while (TargetingModule.HasTarget() && MuzzleLoader.PeekNextShot(deltaTime, out var shot) && HandleShot(shot, target))
            {
                MuzzleLoader.FireShot(deltaTime, shot);
            }
        }

        /// <summary>
        /// Gets the inherited velocity of projectiles based on the selected velocity type.
        /// </summary>
        public Vector3 GetProjectileInheritedVelocity(Vector3 position)
        {
            return VelocityType switch
            {
                VelocityTypes.Rigidbody => Rigidbody ? Rigidbody.GetPointVelocity(position) : default,
                VelocityTypes.Custom => CustomVelocity,
                VelocityTypes.None => default,
                _ => throw new System.NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets the gravity setting for projectiles, either using the override value or defaulting to the ammunition prefab.
        /// </summary>
        public bool GetProjectileHasGravity()
        {
            return OverrideHasGravity.HasValue ? OverrideHasGravity.Value : AmmunitionPrefab?.HasGravity ?? default;
        }

        /// <summary>
        /// Gets the acceleration setting for projectiles, either using the override value or defaulting to the ammunition prefab.
        /// </summary>
        public Vector3 GetProjectileAcceleration()
        {
            return OverrideAcceleration.HasValue ? OverrideAcceleration.Value : AmmunitionPrefab?.Acceleration ?? default;
        }

        /// <summary>
        /// Handles what happens when an interception calculation fails.
        /// </summary>
        private void HandleInterceptFailure(EndEffector endEffector, Trajectory target, float deltaTime)
        {
            OnInterceptFailed.Invoke();

            switch (InterceptFailureBehaviour)
            {
                case InterceptFailBehaviourTypes.AimStraightAtTarget:
                    endEffector.AimAtPosition(target.Position, deltaTime);
                    break;
                case InterceptFailBehaviourTypes.TimeOut:
                    TimeOut(deltaTime);
                    break;
                case InterceptFailBehaviourTypes.ClearTarget:
                    TargetingModule.ClearTarget();
                    break;
            }
        }

        /// <summary>
        /// Attempts to compute the trajectory for the projectile interception.
        /// </summary>
        public bool Intercept(out Trajectory trajectory, Trajectory target, Vector3 position, float spread = default, float tError = default)
        {
            var pInheritedVelocity = GetProjectileInheritedVelocity(position);
            var pHasGravity = GetProjectileHasGravity();
            var pAcceleration = GetProjectileAcceleration();
            return target.Intercept(out trajectory, LaunchVelocity, position, pInheritedVelocity, pHasGravity, pAcceleration, spread, tError, !Mortar);
        }

        /// <summary>
        /// Handles a peeked shot to be fired at the given target trajectory
        /// </summary>
        protected virtual bool HandleShot(Shot peekedShot, Trajectory targetTrajectory)
        {
            // Attempt to compute the projectile trajectory for interception
            if (!InterceptTarget(out var shotTrajectory, peekedShot, targetTrajectory))
            {
                return false; // Interception failed; do not proceed with firing
            }

            // Prepare event arguments for pre-fire validation
            var beforeFireArgs = new BeforeKinematicTurretFiredArgs()
            {
                CancellationToken = new(),
                KinematicTurret = this,
                Muzzle = peekedShot.Muzzle,
                Trajectory = shotTrajectory
            };

            // Invoke the BeforeFired event, allowing external systems to cancel the shot if necessary
            BeforeFired.Invoke(beforeFireArgs);

            // If an external system requested shot cancellation, abort firing
            if (beforeFireArgs.CancellationToken.IsCancellationRequested())
            {
                return false;
            }

            // Fire the ammunition with the calculated trajectory
            LaunchAmmunition(peekedShot, shotTrajectory);
            return true;
        }

        /// <summary>
        /// Computes the trajectory required for a fired projectile to intercept a moving target (f possible)
        /// </summary>
        bool InterceptTarget(out Trajectory shotTrajectory, Shot shot, Trajectory targetTrajectory)
        {
            // If we are NOT converging shots and have a designated aiming end effector
            if (!MuzzleConvergence && MuzzleLoader.AimingEndEffector)
            {
                // Calculate an intercept trajectory relative to the aiming end effector's position
                if (Intercept(out shotTrajectory, targetTrajectory, MuzzleLoader.AimingEndEffector.Position, Spread, shot.TimeShift))
                {
                    // Translate the trajectory's position from the aiming end effector to the muzzle
                    var efRelPosition = MuzzleLoader.AimingEndEffector.transform.InverseTransformPoint(shotTrajectory.Position);
                    shotTrajectory.Position = shot.Muzzle.transform.TransformPoint(efRelPosition);

                    // Adjust velocity to match muzzle-relative transformation
                    var efRelVelocity = MuzzleLoader.AimingEndEffector.transform.InverseTransformDirection(shotTrajectory.Velocity);
                    shotTrajectory.Velocity = shot.Muzzle.transform.TransformDirection(efRelVelocity);

                    // Adjust launch velocity to ensure proper directional alignment
                    var efRelLaunchVelocity = MuzzleLoader.AimingEndEffector.transform.InverseTransformDirection(shotTrajectory.LaunchVelocity);
                    shotTrajectory.LaunchVelocity = shot.Muzzle.transform.TransformDirection(efRelLaunchVelocity);

                    return true;
                }

                // Unable to find a valid intercept. This is exceptionally unlikely but possible at quartic boundaries.
                shotTrajectory = default;
                return false;
            }

            // If shots are converging or we don't have an aiming end effector, intercept directly from the muzzle position
            return Intercept(out shotTrajectory, targetTrajectory, shot.Muzzle.Position, Spread, shot.TimeShift);
        }

        /// <summary>
        /// Instantiates and launches an ammunition projectile based on the provided trajectory.
        /// </summary>
        protected virtual BaseAmmunition LaunchAmmunition(Shot shot, Trajectory trajectory)
        {
            // Determine the initial rotation of the ammunition
            // If shots are converging, align it with the computed launch velocity direction
            // Otherwise, use the existing rotation of the muzzle
            var rotation = MuzzleConvergence
                ? Quaternion.LookRotation(trajectory.LaunchVelocity, shot.Muzzle.transform.up)
                : shot.Muzzle.transform.rotation;

            var position = trajectory.Position;
            var ammunition = PoolingService.Instance.GetOrCreate(AmmunitionPrefab, position, rotation);
            ammunition.Launch(trajectory);
            TargetingModule.CopyTo(ammunition.TargetingModule);
            AmmunitionFired.Invoke(ammunition);
            return ammunition;
        }

        # region Events

        [Tooltip("Invoked whenever a trajectory interception fails.")]
        public UnityEvent OnInterceptFailed = new();

        [Tooltip("Invoked before firing a shot.")]
        public UnityEvent<BeforeKinematicTurretFiredArgs> BeforeFired = new();

        [Tooltip("Invoked when an ammunition has been fired.")]
        public UnityEvent<BaseAmmunition> AmmunitionFired = new();

        # endregion

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public KinematicTurretGizmos KinematicTurretGizmos = new()
        {
            MainColor = AdvancedColors.TransparentWhite
        };

        void OnDrawGizmos()
        {
            if (!ShowGizmos)
            {
                return;
            }

            KinematicTurretGizmos.DrawGizmos(this);
        }
#endif
    }

    /// <summary>
    /// Provides details about a kinematic turret's firing event before the shot is executed.
    /// Allows external systems to cancel the shot if necessary.
    /// </summary>
    public struct BeforeKinematicTurretFiredArgs
    {
        public CancellationToken CancellationToken;
        public KinematicTurret KinematicTurret;
        public Muzzle Muzzle;
        public Trajectory Trajectory;
    }
}
