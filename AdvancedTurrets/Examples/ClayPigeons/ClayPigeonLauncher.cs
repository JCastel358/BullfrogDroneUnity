using AdvancedTurrets.Behaviours.Ammunitions;
using AdvancedTurrets.Libraries;
using AdvancedTurrets.Utilities;
using AdvancedTurrets.Kinematics;
using UnityEngine;
using AdvancedTurrets.Behaviours.Lifecycles;


namespace AdvancedTurrets.Examples.ClayPigeons
{
    [AddComponentMenu("Hidden/")]
    public class ClayPigeonLauncher : MonoBehaviour
    {
        public float RateSec = 3;
        public float Spread = 25;
        public float Velocity = 35;
        public float Duration = 10f;
        public bool HasGravity;
        public Vector3 Acceleration;
        RateLimiter _rateLimiter = new();

        public BaseAmmunition BaseAmmunition;

        void Update()
        {
            if (BaseAmmunition && _rateLimiter.Pass(AdvancedTime.SmartDeltaTime, 1 / RateSec, out var err))
            {
                var trajectory = new Trajectory(transform.position, transform.forward.RandomizeDirection(Spread) * Velocity, HasGravity, Acceleration, Duration, true);
                trajectory.ShiftTime(err);
                var instance = PoolingService.Instance.GetOrCreate(BaseAmmunition, trajectory.Position, Quaternion.LookRotation(trajectory.Velocity));
                instance.Launch(trajectory);
            }
        }
    }
}