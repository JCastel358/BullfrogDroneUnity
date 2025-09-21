using AdvancedTurrets.Serialization;
using AdvancedTurrets.Utilities;
using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Effects
{
    /// <summary>
    /// Simulates recoil by retracting and recovering along the local Z-axis.
    /// </summary>
    public class Recoiler : MonoBehaviour
    {
        [Tooltip("The minimum local Z position the recoiler can retract to.")]
        public float MinZ;

        [Min(0)]
        [Tooltip("The force applied when recoil is triggered.")]
        public float RecoilMagnitude = 1f;

        [Tooltip("The remaining recoil force that the recoiler is retracting from.")]
        public float RemainingRecoil;

        [Min(0)]
        [Tooltip("The rate at which recoil force diminishes over time.")]
        public float Dampening = 0.5f;

        [Min(0)]
        [Tooltip("The rate at which the recoiler returns to its resting position.")]
        public float Recovery = 2f;

        [Tooltip("The default local position of the recoiler when at rest.")]
        public AdvancedNullable<Vector3> LocalRestPosition = new();

        private void Start()
        {
            LocalRestPosition.SetIfEmpty(transform.localPosition);
        }

        private void Update()
        {
            var deltaTime = AdvancedTime.SmartDeltaTime;

            RemainingRecoil = Mathf.Lerp(RemainingRecoil, 0, deltaTime * Dampening);
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                LocalRestPosition.GetValueOrDefault() + Vector3.back * RemainingRecoil,
                deltaTime * Recovery);

            if (transform.localPosition.z < MinZ)
            {
                RecoilMaxed.Invoke(RemainingRecoil);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, MinZ);
                RemainingRecoil = 0;
            }
        }

        public void Recoil()
        {
            RemainingRecoil += RecoilMagnitude;
        }

        [Tooltip("Invoked when the recoiler has fully retracted to 'MinZ'. The float parameter represents the remaining recoil magnitude.")]
        public UnityEvent<float> RecoilMaxed = new();
    }
}
