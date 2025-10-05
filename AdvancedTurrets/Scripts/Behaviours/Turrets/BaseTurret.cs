using AdvancedTurrets.Behaviours.Actuators;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Utilities;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Turrets
{
    /// <summary>
    /// The base class for all AdvancedTurrets, containing common settings and configurations shared across all turret implementations.
    /// </summary>
    public class BaseTurret : MonoBehaviour
    {
        [Tooltip("True: Ammunitions will always converge towards the target. False: Muzzles will fire ammunitions straight along their forward axis.")]
        public bool MuzzleConvergence = true;

        [Tooltip("The clock responsible for tracking timeout periods (e.g., when no target is available).")]
        public CheckTimer TimeoutClock = new();

        [SerializeField]
        [Tooltip("The MuzzleLoader responsible for handling cycletime and muzzle management.")]
        private LazyComponent<MuzzleLoader> _lazyMuzzleLoader = new(ComponentAncestry.InChildren);
        public MuzzleLoader MuzzleLoader => _lazyMuzzleLoader.Get(this);

        [SerializeField]
        [Tooltip("The TargetingModule that will be used by derived turrets.")]
        private LazyComponent<TargetingModule> _lazyTargetingModule = new(ComponentAncestry.InParent);
        public TargetingModule TargetingModule => _lazyTargetingModule.Get(this);

        /// <summary>
        /// Returns to center once the appropriate timeout duration has elapsed.
        /// </summary>
        protected bool TimeOut(float deltaTime)
        {
            return TimeoutClock.HasElapsed() && ReturnToCenter(deltaTime);
        }

        /// <summary>
        /// Rotates all child actuators back to their centered rotations.
        /// </summary>
        protected bool ReturnToCenter(float deltaTime)
        {
            var isCentered = true;

            foreach (var actuator in GetComponentsInChildren<Actuator>())
            {
                isCentered &= actuator.ReturnToCenter(deltaTime);
            }

            return isCentered;
        }
    }
}
