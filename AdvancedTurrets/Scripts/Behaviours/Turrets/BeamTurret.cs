using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Geometrics;
using AdvancedTurrets.Visualization;
using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Behaviours.Lifecycles;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// A turret implementation that fires beams using linear raycasting (hitscan).
    /// Includes collision detection and various raycasting modes.
    /// </summary>
    public class BeamTurret : BaseTurret
    {
        [Tooltip("Prefab for the beam that will be instantiated when the turret fires a muzzle.")]
        public BaseBeam BeamPrefab;

        [Tooltip("The maximum beam range of this turret.")]
        public float MaxRange = 10000f;

        [Tooltip("Whether or not the turret will fire towards a target even if it's out of range.")]
        public bool FireWhenOutOfRange = false;

        [Tooltip("The collision mask used for collider raycasting.")]
        public LayerMask CollisionMask = -1;

        public enum BeamRaycastTypes
        {
            StopAtFirstHit,
            StopAtTarget,
            StopAtRange
        }

        [Tooltip("Determines how the beam interacts with colliders when raycasting.")]
        public BeamRaycastTypes RaycastType = BeamRaycastTypes.StopAtFirstHit;

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

            TimeoutClock.Reset();

            var aimPosition = target.Position;
            if (!endEffector.AimAtPosition(aimPosition, deltaTime))
            {
                return;
            }

            if (!FireWhenOutOfRange && !IsInRangeOf(target.Position))
            {
                return;
            }

            while (TargetingModule.HasTarget() && MuzzleLoader.PeekNextShot(deltaTime, out var shot) && HandleShot(shot, target))
            {
                MuzzleLoader.FireShot(deltaTime, shot);
            }
        }

        /// <summary>
        /// Attempts to fire a peeked shot. Can be cancelled if anyone requests cancellation from the <see cref="BeforeFired"/> event. 
        /// </summary>
        protected virtual bool HandleShot(Shot peekedShot, Trajectory targetTrajectory)
        {
            var beforeFireArgs = new BeforeBeamTurretFiredArgs
            {
                CancellationToken = new(),
                BeamTurret = this,
                Muzzle = peekedShot.Muzzle
            };

            BeforeFired.Invoke(beforeFireArgs);

            if (beforeFireArgs.CancellationToken.IsCancellationRequested())
            {
                return false;
            }

            var raycastHits = GetRaycastHits(peekedShot.Muzzle, targetTrajectory.Position, out var beamLine);
            FireBeam(peekedShot.Muzzle, beamLine, raycastHits);
            return true;
        }

        /// <summary>
        /// Collects hit objects along the beam path depending on raycasting type.
        /// </summary>
        protected IEnumerable<RaycastHit> GetRaycastHits(Muzzle muzzle, Vector3 targetPosition, out Line beamLine)
        {
            var beamDirection = MuzzleConvergence ? targetPosition - muzzle.Position : muzzle.Forward;
            var range = GetRaycastRange(muzzle, targetPosition);
            beamLine = new Line(muzzle.Position, muzzle.Position + beamDirection.normalized * range);

            switch (RaycastType)
            {
                case BeamRaycastTypes.StopAtFirstHit:
                    if (beamLine.Raycast(CollisionMask, out var raycastHit))
                    {
                        beamLine.To = raycastHit.point;
                        return new[] { raycastHit };
                    }
                    break;
                case BeamRaycastTypes.StopAtTarget:
                case BeamRaycastTypes.StopAtRange:
                    return beamLine.RaycastAll(CollisionMask);
                default:
                    throw new System.NotImplementedException();
            }

            return new RaycastHit[0];
        }

        /// <summary>
        /// Determines beam length based upon raycasting type.
        /// </summary>
        internal float GetRaycastRange(EndEffector endEffector, Vector3 targetPosition)
        {
            return RaycastType switch
            {
                BeamRaycastTypes.StopAtFirstHit => MaxRange,
                BeamRaycastTypes.StopAtTarget => (targetPosition - endEffector.Position).magnitude,
                BeamRaycastTypes.StopAtRange => MaxRange,
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// Invoked when a beam has been fired along the given line with a reference to the raycasted colliders.
        /// </summary>
        protected virtual void FireBeam(Muzzle muzzle, Line line, IEnumerable<RaycastHit> raycastHits)
        {
            if (!BeamPrefab)
            {
                Debug.LogWarning($"No beam prefab specified on {name}!");
                return;
            }

            var beamPosition = muzzle.transform.position;
            var beamRotation = Quaternion.LookRotation(line.Direction, muzzle.transform.up);
            var beam = PoolingService.Instance.GetOrCreate(BeamPrefab, beamPosition, beamRotation);
            beam.Fire(line, raycastHits);
            RaycastHits.Invoke(raycastHits);
        }

        /// <summary>
        /// Checks whether the target is within firing range.
        /// </summary>
        protected virtual bool IsInRangeOf(Vector3 target)
        {
            return Vector3.Distance(target, transform.position) <= MaxRange;
        }

        # region Events

        [Tooltip("Invoked before firing a shot. Listeners can optionally request cancellation.")]
        public UnityEvent<BeforeBeamTurretFiredArgs> BeforeFired = new();

        [Tooltip("Invoked with the raycasted colliders hit when firing a beam.")]
        public UnityEvent<IEnumerable<RaycastHit>> RaycastHits = new();

        #endregion

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public BeamTurretGizmos BeamTurretGizmos = new();

        void OnDrawGizmos()
        {
            if (!ShowGizmos)
            {
                return;
            }

            BeamTurretGizmos.DrawGizmos(this);
        }
#endif
    }

    /// <summary>
    /// Provides details about a beam turret's firing event before the shot is executed.
    /// Allows external systems to cancel the shot if necessary.
    /// </summary>
    public struct BeforeBeamTurretFiredArgs
    {
        public CancellationToken CancellationToken;
        public BeamTurret BeamTurret;
        public Muzzle Muzzle;
    }
}
