using AdvancedTurrets.Mathematics;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Actuators
{
    /// <summary>
    /// Input args for actuation. Caches components/parameters that will be used across one to many actuators.
    /// </summary>
    public class ActuationArgs
    {
        public Transform EndEffector;
        public Vector3 EndEffectorRelativeForward;
        public Vector3 TargetPosition;
        public float DeltaTime;
        public float MaxAngle;

        public ActuationArgs(Transform endEffector, Vector3 endEffectorRelativeForward, Vector3 targetPosition, float deltaTime, float maxAngle)
        {
            EndEffector = endEffector;
            EndEffectorRelativeForward = endEffectorRelativeForward;
            TargetPosition = targetPosition;
            DeltaTime = deltaTime;
            MaxAngle = maxAngle;
        }

        /// <summary>
        /// The angle between the end effector's current and designed forward direction.
        /// </summary>
        public float GetAngle()
        {
            var currentForward = EndEffector.rotation * EndEffectorRelativeForward;
            var desiredForward = TargetPosition - EndEffector.position;
            return Vector3.Angle(currentForward, desiredForward);
        }

        /// <summary>
        /// Determines whether the actuator is correctly aimed at the target within the allowed angle range.
        /// </summary>
        public bool IsAimed()
        {
            return MaxAngle.GreaterThanOrEqual(GetAngle());
        }
    }
}
