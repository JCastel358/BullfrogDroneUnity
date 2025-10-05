using UnityEngine;


namespace AdvancedTurrets.Behaviours.Actuators
{
    /// <summary>
    /// Represents the current state of an actuator, including its central (origin) rotation and rotation axis.
    /// </summary>
    public class ActuationState
    {
        public Quaternion CenterRotation;
        public Vector3 RotationAxis;
        public ActuationState(Quaternion centerRotation, Vector3 rotationAxis)
        {
            RotationAxis = rotationAxis;
            CenterRotation = centerRotation;
        }
    }
}
