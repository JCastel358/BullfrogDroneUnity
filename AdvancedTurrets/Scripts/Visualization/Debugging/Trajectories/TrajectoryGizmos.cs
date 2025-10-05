using System;
using AdvancedTurrets.Kinematics;
using UnityEngine;


namespace AdvancedTurrets.Visualization.Trajectories
{
    [Serializable]
    public class TrajectoryGizmos
    {
        [Header("Self")]
        public InterceptFrameGizmos ExpirationFrame = new()
        {
            MainColor = Color.green
        };
        public TerminalPointArgs ExpirationPosition = new()
        {
            MainColor = AdvancedColors.LightRed
        };
        public ElapsedTrajectoryGizmos ElapsedLines = new()
        {
            MainColor = AdvancedColors.LightGray
        };
        public ElapsedTrajectoryFixedPointsGizmos ElapsedFixedPoints = new()
        {
            MainColor = AdvancedColors.DarkGray
        };
        public TrajectoryExpectedPositionArgs ExpectedPosition = new()
        {
            MainColor = Color.white
        };
        public RemainingTrajectoryGizmos RemainingLines = new()
        {
            MainColor = AdvancedColors.LightGreen,
        };
        public RemainingTrajectoryFixedPointsGizmos RemainingFixedPoints = new()
        {
            MainColor = AdvancedColors.DarkGray
        };
        public TrajectoryTimeRemainingGizmos TimeRemaining = new()
        {
            MainColor = Color.green,
            ElapsedColor = AdvancedColors.LightGray
        };

        [Header("Target")]
        public InterceptFrameGizmos TargetExpirationFrame = new()
        {
            MainColor = Color.white,
        };
        public TerminalPointArgs TargetExpirationPosition = new()
        {
            MainColor = AdvancedColors.LightRed
        };
        public ElapsedTrajectoryGizmos TargetElapsedLines = new()
        {
            MainColor = AdvancedColors.LightGray,
        };
        public ElapsedTrajectoryFixedPointsGizmos TargetElapsedFixedPoints = new()
        {
            MainColor = AdvancedColors.DarkGray
        };
        public TrajectoryExpectedPositionArgs TargetExpectedPosition = new()
        {
            MainColor = Color.white
        };
        public RemainingTrajectoryGizmos TargetRemainingLines = new()
        {
            MainColor = AdvancedColors.TransparentWhite,
        };
        public RemainingTrajectoryFixedPointsGizmos TargetRemainingFixedPoints = new()
        {
            MainColor = AdvancedColors.DarkGray
        };

        public void DrawGizmos(Trajectory trajectory)
        {
            // Self
            TimeRemaining.DrawGizmos(trajectory);
            ExpirationFrame.DrawGizmos(trajectory);
            ExpirationPosition.DrawGizmos(trajectory);
            ElapsedLines.DrawGizmos(trajectory);
            ElapsedFixedPoints.DrawGizmos(trajectory);
            ExpectedPosition.DrawGizmos(trajectory);
            RemainingLines.DrawGizmos(trajectory);
            RemainingFixedPoints.DrawGizmos(trajectory);

            // Target
            TargetExpirationFrame.DrawGizmos(trajectory.TargetTrajectory, trajectory);
            TargetExpirationPosition.DrawGizmos(trajectory.TargetTrajectory, trajectory);
            TargetElapsedLines.DrawGizmos(trajectory.TargetTrajectory, trajectory);
            TargetElapsedFixedPoints.DrawGizmos(trajectory.TargetTrajectory, trajectory);
            TargetExpectedPosition.DrawGizmos(trajectory.TargetTrajectory);
            TargetRemainingLines.DrawGizmos(trajectory.TargetTrajectory, trajectory);
            TargetRemainingFixedPoints.DrawGizmos(trajectory.TargetTrajectory, trajectory);
        }
    }
}