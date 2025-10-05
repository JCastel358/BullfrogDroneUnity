using System;
using AdvancedTurrets.Kinematics;
using AdvancedTurrets.Mathematics;
using AdvancedTurrets.Utilities;
using UnityEditor;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class TrajectoryTimeRemainingGizmos : BaseGizmos
    {
        [Tooltip("If true, the remaining time will be rounded to the nearest fixed time step.")]
        public bool RoundToFixedTime = false;

        [Tooltip("The color used to represent the elapsed portion of the time.")]
        public Color ElapsedColor = AdvancedColors.LightGray;

        [Tooltip("The font size used for displaying the remaining time.")]
        public int FontSize = 30;

        public void DrawGizmos(Trajectory trajectory)
        {
            // Ensure the function only executes if enabled and the trajectory is valid.
            if (!Enabled || trajectory == default)
            {
                return;
            }

            // Calculate the scalar size adjustment based on distance from the camera.
            var scalar = 1 / HandleUtility.GetHandleSize(trajectory.GetTerminalPoint());

            // Retrieve the remaining time of the trajectory.
            var timeRemainingString = trajectory.GetRemainingTime();

            // Round to the nearest fixed time step if enabled.
            if (RoundToFixedTime)
            {
                timeRemainingString = AdvancedMathematics.ToIncrementalCeil(timeRemainingString, AdvancedTime.FixedDeltaTime);
            }

            // Determine the color based on whether the trajectory has elapsed or still has remaining time.
            var textColor = trajectory.GetRemainingTime() < 0 ? ElapsedColor : MainColor;

            // Display the remaining time as a label at the intercept point.
            Handles.Label(trajectory.GetTerminalPoint() + Vector3.up, $"{timeRemainingString:N3}s", new GUIStyle()
            {
                fontSize = Mathf.CeilToInt(scalar * FontSize),
                normal = new GUIStyleState()
                {
                    textColor = textColor
                },
                alignment = TextAnchor.MiddleCenter
            });
        }
    }
}