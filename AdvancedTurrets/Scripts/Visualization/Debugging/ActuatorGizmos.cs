using System;
using AdvancedTurrets.Behaviours.Actuators;
using UnityEngine;

namespace AdvancedTurrets.Visualization
{
    [Serializable]
    public class ActuatorGizmos : BaseGizmos
    {
        [Tooltip("Indicates whether the rotation axis should be displayed in the Gizmo view.")]
        public bool ShowRotationAxis = true;

        [Tooltip("Indicates whether the boundaries of the actuator should be shown in the Gizmo view.")]
        public bool ShowBoundaries = true;

        [Tooltip("Indicates whether the center line of the actuator should be drawn in the Gizmo view.")]
        public bool ShowCenterLine = true;

        [Tooltip("Indicates whether the current line (representing actuator movement) should be shown in the Gizmo view.")]
        public bool ShowCurrentLine = true;

        [Tooltip("The radius of the actuator visualization in the Gizmo view.")]
        public float Radius = 3f;

        [Tooltip("The number of lines used to represent the radius in the Gizmo view.")]
        public int RadiusLines = 25;

        public void DrawGizmos(Actuator actuator)
        {
            if (!Enabled || actuator == default)
            {
                return;
            }

            var actuatorState = actuator.GetActuationState();
            var centerRotation = actuatorState.CenterRotation;
            var rotationAxis = actuatorState.RotationAxis;

            actuator.transform.GetPositionAndRotation(out var position, out var rotation);

            // Draw the rotation axis if enabled in the arguments
            Gizmos.color = MainColor;
            if (ShowRotationAxis)
            {
                var axisFrom = position + rotationAxis * Radius;
                var axisTo = position - rotationAxis * Radius;
                AdvancedGizmos.DrawDottedLine(axisFrom, axisTo);
            }

            // Draw the center line if enabled in the arguments
            if (ShowCenterLine)
            {
                var centerRadialPoint = position + centerRotation * Vector3.forward * Radius;
                AdvancedGizmos.DrawDottedLine(position, centerRadialPoint);
            }

            // Draw the boundaries of the actuator if enabled in the arguments
            if (ShowBoundaries)
            {
                var degreesSpan = actuator.MaxDegrees - actuator.MinDegrees;
                var degreesFromCenter = degreesSpan / 2;
                var circleNormal = centerRotation * actuator.RelativeAxis.normalized;
                var circleUp = centerRotation * Vector3.forward;
                var circleOffsetDegrees = 90 - degreesFromCenter;

                if (actuator.LimitRotation)
                {
                    // Draw the boundary circle in red if rotation is limited
                    Gizmos.color = AdvancedColors.LightRed;
                    AdvancedGizmos.DrawCircle(position, circleNormal, circleUp, Radius, RadiusLines, 360 - degreesSpan, true, circleOffsetDegrees);

                    // Draw lines to represent the minimum and maximum radial points
                    var angleAxis = Quaternion.AngleAxis(degreesFromCenter, actuator.RelativeAxis.normalized);

                    Gizmos.color = Color.green;
                    var minRadialPoint = position + centerRotation * Quaternion.Inverse(angleAxis) * Vector3.forward * Radius;
                    Gizmos.DrawLine(position, minRadialPoint);

                    var maxRadialPoint = position + centerRotation * angleAxis * Vector3.forward * Radius;
                    Gizmos.DrawLine(position, maxRadialPoint);

                    // Draw the inner boundary of the actuator
                    AdvancedGizmos.DrawCircle(position, circleNormal, circleUp, Radius, Mathf.CeilToInt(RadiusLines * degreesSpan / 360f), degreesSpan, false, circleOffsetDegrees);
                }
                else
                {
                    // Draw the boundary circle in green if rotation is not limited
                    Gizmos.color = AdvancedColors.LightGreen;
                    AdvancedGizmos.DrawCircle(position, circleNormal, circleUp, Radius, RadiusLines, 360, true, circleOffsetDegrees);
                }
            }

            // Draw the current line if enabled in the arguments
            if (ShowCurrentLine)
            {
                Gizmos.color = Color.white;
                var currentRadialPosition = position + rotation * Vector3.forward * Radius;
                AdvancedGizmos.DrawDottedLine(position, currentRadialPosition);
            }
        }
    }
}