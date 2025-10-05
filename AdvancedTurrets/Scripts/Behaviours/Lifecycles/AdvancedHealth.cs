using AdvancedTurrets.Utilities;
using AdvancedTurrets.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using AdvancedTurrets.Visualization;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Manages health with optional layered stacking through recursive parent instances.
    /// </summary>
    public class AdvancedHealth : MonoBehaviour
    {
        [Min(0)]
        [Tooltip("Maximum health capacity.")]
        public float Capacity = 1000;

        [Range(0, 1)]
        [Tooltip("Current health percentage.")]
        public float Percent = 1f;

        [Tooltip("Health regeneration rate per second. Negative values cause deterioration.")]
        public float RegenPerSec = 0;

        [Tooltip("Indicates whether this health instance is active. If false, the entity does not take damage but can still regenerate")]
        public bool AvailableForDamage = true;

        [Tooltip("Optional parent health component for recursive damage delegation.")]
        public AdvancedHealth Parent;

        public float Remaining => Capacity * Percent;

        public float TotalCapacity => Capacity + (Parent ? Parent.TotalCapacity : 0);

        public float TotalRemaining => Capacity * Percent + (Parent ? Parent.TotalRemaining : 0);

        public float TotalPercent => TotalRemaining / TotalCapacity;

        public float TotalRegen => RegenPerSec + (Parent ? Parent.TotalRegen : 0);

        // Not directly referenced but leveraged by UnityEvents
        public void ToggleDamageAvailability(bool availableForDamage)
        {
            AvailableForDamage = availableForDamage;
        }

        private void Update()
        {
            if (RegenPerSec != 0)
            {
                var regenPercent = RegenPerSec / Capacity * AdvancedTime.SmartDeltaTime;
                Percent = Mathf.Clamp(Percent + regenPercent, 0, 1);
            }

            if (Percent.Approximately(0))
            {
                Depleted.Invoke();
            }
            else if (Percent.Approximately(1))
            {
                Replenished.Invoke();
            }
        }

        public bool TakeDamage(float damage, out float overkill, AdvancedHealth sender = default, bool skipParent = false)
        {
            if (!skipParent && Parent && Parent.enabled && Parent.gameObject.activeSelf)
            {
                Parent.TakeDamage(damage, out damage, sender, skipParent);
            }

            if (!AvailableForDamage)
            {
                overkill = damage;
                return false;
            }

            var absorbed = Mathf.Min(Remaining, damage);
            Percent = Mathf.Clamp01(Percent - absorbed / Capacity);
            overkill = damage - absorbed;

            if (Percent.Approximately(0f))
            {
                Depleted.Invoke();
            }

            return overkill <= 0;
        }

        [Tooltip("Invoked whenever health has been depleted (0%)")]
        public UnityEvent Depleted = new();

        [Tooltip("Invoked whenever health has been regenerated fully (100%)")]
        public UnityEvent Replenished = new();

#if UNITY_EDITOR
        [Header("Gizmos")]
        public bool ShowGizmos = true;

        public AdvancedHealthGizmos AdvancedHealthGizmos = new()
        {
            MainColor = Color.green
        };

        private void OnDrawGizmos()
        {
            if (ShowGizmos)
            {
                AdvancedHealthGizmos.DrawGizmos(this);
            }
        }
#endif
    }
}