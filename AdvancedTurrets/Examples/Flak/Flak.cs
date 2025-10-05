using AdvancedTurrets.Utilities;
using AdvancedTurrets.Kinematics;
using UnityEngine;
using UnityEngine.Events;
using AdvancedTurrets.Serialization;

namespace AdvancedTurrets.Examples.Flak
{
    [AddComponentMenu("Hidden/")]
    public class Flak : MonoBehaviour
    {
        [Range(0, 1)] public float DurationVariance = 0.15f;

        [SerializeField] AdvancedNullable<float> _fuseTime = new();

        public void OnProjectileLaunched(Trajectory trajectory)
        {
            _fuseTime = trajectory.Duration + trajectory.Duration * Random.Range(-DurationVariance, DurationVariance);
        }

        private void Update()
        {
            if (_fuseTime.TryGet(out var fuseTime))
            {
                fuseTime -= AdvancedTime.SmartDeltaTime;
                _fuseTime.Set(fuseTime);
                if (_fuseTime <= 0)
                {
                    OnFuseExpired.Invoke();
                }
            }
        }

        public UnityEvent OnFuseExpired = new();
    }
}