using System.Linq;
using AdvancedTurrets.Mathematics;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Visualization;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Actuators
{
    /// <summary>
    /// Controls rotation of its transform by rotating hierarchical actuators to aim towards itself towards designated targets.
    /// </summary>
    /// <remarks>
    /// Non-hierarchical actuators are skipped and logged as warnings since their rotation won't change anything.
    /// </remarks>
    public class EndEffector : MonoBehaviour
    {
        [Tooltip("Iterate parent actuators in reverse order (top-down).")]
        public bool InverseOrder = false;

        [SerializeField]
        [Tooltip("Parent actuators used to rotate this EndEffector.")]
        private LazyComponents<Actuator> _lazyActuators = new(ComponentAncestry.InParent);
        public Actuator[] ParentActuators => _lazyActuators.Get(this);

        [Tooltip("Local forward direction used for aiming towards the target.")]
        public Vector3 RelativeForward = Vector3.forward;

        [Tooltip("Angle threshold to consider itself aimed towards the target.")]
        public float RequiredAimAngle = 0.05f;

        [Tooltip("Stop any further parent actuation once aimed at target.")]
        public bool StopOnAimed = true;

        public Vector3 Position => transform.position;

        public Vector3 Forward => transform.rotation * RelativeForward;

        /// <summary>
        /// Returns true if the relative forward of this is looking at the target position within <see cref="RequiredAimAngle"/>
        /// </summary>
        private bool IsAimed(Vector3 targetPosition)
        {
            var angle = Vector3.Angle(Forward, targetPosition - Position);
            return angle.LessThanOrEqual(RequiredAimAngle);
        }

        /// <summary>
        /// Rotates the EndEffector toward a target position using its parent actuators.
        /// </summary>
        /// <remarks>
        /// Iteratively adjusts actuators until aimed or all actuators have been processed.
        /// Stops early if <see cref="StopOnAimed"/> is enabled.
        /// </remarks>
        public bool AimAtPosition(Vector3 targetPosition, float deltaTime)
        {
            var actuationArgs = new ActuationArgs(transform, RelativeForward, targetPosition, deltaTime, RequiredAimAngle);

            var actuators = InverseOrder ? ParentActuators.Reverse() : ParentActuators;

            foreach (var actuator in actuators)
            {
                if (transform.IsChildOf(actuator.transform))
                {
                    actuator.AimAtTarget(actuationArgs);

                    if (StopOnAimed && IsAimed(targetPosition))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.LogWarning($"EndEffector '{transform}' is not a child of actuator '{actuator}'. Skipped.");
                }
            }

            return IsAimed(targetPosition);
        }

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public EndEffectorGizmos EndEffectorGizmos = new();

        private void OnDrawGizmos()
        {
            if (!ShowGizmos)
            {
                return;
            }

            EndEffectorGizmos.DrawGizmos(this);
        }
#endif
    }
}
