using AdvancedTurrets.Serialization;
using AdvancedTurrets.Utilities;
using UnityEngine;
using UnityEngine.Events;


namespace AdvancedTurrets.Behaviours.Lifecycles
{
    /// <summary>
    /// Detects when a <see cref="TrailRenderer"/> has stopped moving and invokes an event.
    /// </summary>
    public class TrailRendererStopRelay : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The TrailRenderer component to monitor.")]
        private LazyComponent<TrailRenderer> _lazyTrailRenderer = new(ComponentAncestry.OnSelf);
        public TrailRenderer TrailRenderer => _lazyTrailRenderer.Get(this);

        private Vector3 _lastPosition;
        private float? _stoppedAt;

        private void Update()
        {
            if (transform.position != _lastPosition)
            {
                _stoppedAt = null;
            }
            else if (_stoppedAt.HasValue)
            {
                if (AdvancedTime.Time - _stoppedAt.Value >= TrailRenderer.time)
                {
                    Stopped.Invoke();
                }
            }
            else
            {
                _stoppedAt = AdvancedTime.Time;
            }

            _lastPosition = transform.position;
        }

        [Tooltip("Invoked when the TrailRenderer stops moving for the duration of its trail time.")]
        public UnityEvent Stopped = new();
    }
}
