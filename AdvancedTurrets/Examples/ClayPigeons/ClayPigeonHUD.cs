using AdvancedTurrets.Utilities;
using AdvancedTurrets.Kinematics;
using UnityEngine;
using AdvancedTurrets.Behaviours.Collisions;
using AdvancedTurrets.Serialization;
using AdvancedTurrets.Behaviours.Lifecycles;

namespace AdvancedTurrets.Examples.ClayPigeons
{
    [AddComponentMenu("Hidden/")]
    [RequireComponent(typeof(DynamicColliderGroup))]
    public class ClayPigeonHUD : MonoBehaviour
    {
        public ClayPigeon ClayPigeonPrefab;
        public float Duration = 10f;
        public float LaunchVelocityMagnitude = 100;
        public bool HasGravity = true;
        public Vector3 Acceleration = default;
        public Camera Camera;

        public float FireRate = 30;
        [SerializeField] RateLimiter _rateLimiter = new();

        [SerializeField] LazyComponent<DynamicColliderGroup> _lazyDynamicColliderGroup = new(ComponentAncestry.OnSelf);
        public DynamicColliderGroup DynamicColliderGroup => _lazyDynamicColliderGroup.Get(this);

        void Update()
        {
            LaunchVelocityMagnitude += Input.mouseScrollDelta.y * 5;
            var trajectory = GetTrajectory();
            if (Input.GetMouseButton(0) && _rateLimiter.Pass(AdvancedTime.SmartDeltaTime, 1 / FireRate, out var e))
            {
                var trajectoryCopy = new Trajectory(trajectory.Position, trajectory.Velocity, trajectory.HasGravity, trajectory.Acceleration, Duration, true, null);
                trajectoryCopy.ShiftTime(e);

                var launchRotation = Quaternion.LookRotation(trajectoryCopy.Velocity);
                var clayPigeon = PoolingService.Instance.GetOrCreate(ClayPigeonPrefab, trajectoryCopy.Position, launchRotation);
                DynamicColliderGroup.IgnoreCollisions(clayPigeon.Projectile);
                clayPigeon.Projectile.Launch(trajectoryCopy);
            }
        }

        Vector3 GetLaunchVelocity()
        {
            // Get the world position on the far clipping plane
            var screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.farClipPlane);
            var farClipPosition = Camera.ScreenToWorldPoint(screenPosition);
            return (farClipPosition - GetLaunchPosition()).normalized * LaunchVelocityMagnitude;
        }

        Vector3 GetLaunchPosition()
        {
            return Camera.transform.position;
        }

        Trajectory GetTrajectory()
        {
            var launchVelocity = GetLaunchVelocity();
            var position = GetLaunchPosition();
            return new Trajectory(position, launchVelocity, HasGravity, Acceleration, 10, true);
        }
    }
}