using AdvancedTurrets.Mathematics;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Visualization;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Actuators
{
    /// <summary>
    /// Controls constrained rotation of a transform (notably end effector) towards a target position.
    /// Useful for realistic turret aiming behaviors.
    /// </summary>
    public class Actuator : MonoBehaviour
    {
        public enum ActuationTypes
        {
            Yaw,
            Pitch,
        }

        [Tooltip("Rotation speed in degrees per second.")]
        public float RotationSpeed = 75;

        [Tooltip("Enable min/max rotation limits.")]
        public bool LimitRotation = false;

        [Tooltip("Minimum local rotation (degrees) along rotation axis from centered rotation.")]
        public float MinDegrees = -60;

        [Tooltip("Maximum local rotation (degrees) along rotation axis from centered rotation.")]
        public float MaxDegrees = 60;

        [SerializeField]
        [Tooltip("Central rotation used to measure rotation constraints from.")]
        private AdvancedNullable<Quaternion> _localRotationCenter = new(Quaternion.identity, false);

        [Tooltip("Yaw rotates horizontally (around local y axis), Pitch rotates vertically (around local x axis).")]
        public ActuationTypes ActuationType = ActuationTypes.Yaw;

        public Vector3 RelativeAxis => ActuationType == ActuationTypes.Yaw ? Vector3.up : Vector3.right;

        private void Start()
        {
            _localRotationCenter.SetIfEmpty(transform.localRotation);
        }

        /// <summary>
        /// Smoothly returns actuator to its neutral (centered) rotation.
        /// </summary>
        public bool ReturnToCenter(float deltaTime)
        {
            var targetRotation = _localRotationCenter.GetValueOrDefault();
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, RotationSpeed * deltaTime);
            return AdvancedMathematics.Approximately(0, Quaternion.Angle(transform.localRotation, targetRotation));
        }

        /// <summary>
        /// Rotates actuator towards specified target, respecting rotation constraints if applicable.
        /// </summary>
        public bool AimAtTarget(ActuationArgs args)
        {
            var actuationState = GetActuationState();

            var pointForward = args.EndEffector.rotation * args.EndEffectorRelativeForward;
            var axialDeltaRotation = AdvancedMathematics.RotateAroundAxis(transform.position, transform.rotation, actuationState.RotationAxis, args.EndEffector.position, pointForward, args.TargetPosition);

            var targetRotation = LimitRotation
                ? Quaternion.RotateTowards(actuationState.CenterRotation, axialDeltaRotation.ToRotation, (MaxDegrees - MinDegrees) / 2)
                : axialDeltaRotation.ToRotation;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * args.DeltaTime);
            return args.IsAimed();
        }

        public ActuationState GetActuationState()
        {
            var parentRotation = transform.parent ? transform.parent.rotation : Quaternion.identity;
            var centerRotation = parentRotation * _localRotationCenter.GetValueOrElse(() => transform.localRotation);

            if (LimitRotation)
            {
                centerRotation *= Quaternion.Euler(RelativeAxis.normalized * (MaxDegrees + MinDegrees) / 2);
            }

            var rotationAxis = centerRotation * RelativeAxis.normalized;
            return new(centerRotation, rotationAxis);
        }

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public ActuatorGizmos ActuatorGizmos = new()
        {
            MainColor = AdvancedColors.LightGray
        };

        private void OnDrawGizmos()
        {
            if (!ShowGizmos)
            {
                return;
            }

            ActuatorGizmos.DrawGizmos(this);
        }
#endif
    }
}
