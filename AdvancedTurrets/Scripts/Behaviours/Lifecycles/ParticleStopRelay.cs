using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Relays the <see cref="OnParticleSystemStopped"/> event, allowing external components to react when a particle system stops.
    /// </summary>
    public class ParticleStopRelay : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            Stopped.Invoke();
        }

        [Tooltip("Invoked when the particle system stops playing.")]
        public UnityEvent Stopped = new();
    }
}