using System.Collections;
using System.Collections.Generic;
using AdvancedTurrets.Utilities;
using AdvancedTurrets.Geometrics;
using UnityEngine;
using UnityEngine.Events;
using AdvancedTurrets.Serialization;


namespace AdvancedTurrets.Behaviours.Ammunitions
{
    /// <summary>
    /// Animates a <see cref="LineRenderer"/> component to visualize a beam effect with configurable width and alpha over its lifetime.
    /// </summary>
    public class LineRendererBeam : BaseBeam
    {
        [Tooltip("The duration (in seconds) for which the beam effect is animated.")]
        public float Duration = 0.3f;

        [Tooltip("Determines how the width of the beam changes over its animation lifetime.")]
        public AnimationCurve WidthOverLifetime = new(new Keyframe[] { new(0, .1f), new(.125f, .33f), new(1f, 0f) });

        [Tooltip("Determines how the transparency of the beam changes over its animation lifetime.")]
        public AnimationCurve AlphaOverLifetime = new(new Keyframe[] { new(0, 1f), new(1, .25f) });

        [SerializeField]
        [Tooltip("The LineRenderer component that will be animated.")]
        private LazyComponent<LineRenderer> _lazyLineRenderer = new(ComponentAncestry.InParent);
        public LineRenderer LineRenderer => _lazyLineRenderer.Get(this);

        private Coroutine _animateBeamCoroutine;

        public override void Fire(Line line, IEnumerable<RaycastHit> raycastHits)
        {
            base.Fire(line, raycastHits);

            if (_animateBeamCoroutine != default)
            {
                StopCoroutine(nameof(AnimateBeam));
            }

            _animateBeamCoroutine = StartCoroutine(AnimateBeam(line.Magnitude));
        }

        private IEnumerator AnimateBeam(float range)
        {
            // Set initial beam position and enable rendering
            LineRenderer.SetPositions(new[] { Vector3.zero, Vector3.forward * range });
            LineRenderer.enabled = true;

            var start = AdvancedTime.Time;

            var startColor = LineRenderer.startColor;
            var endColor = LineRenderer.endColor;

            while (true)
            {
                var elapsed = AdvancedTime.Time - start;
                if (elapsed > Duration)
                {
                    break;
                }

                var eval = elapsed / Duration;

                // Update alpha (transparency) over time
                var alpha = AlphaOverLifetime.Evaluate(eval * AlphaOverLifetime.keys[^1].time);
                startColor.a = alpha;
                endColor.a = alpha;
                LineRenderer.startColor = startColor;
                LineRenderer.endColor = endColor;

                // Update beam width over time
                LineRenderer.widthMultiplier = WidthOverLifetime.Evaluate(eval * WidthOverLifetime.keys[^1].time);
                yield return null;
            }

            // Disable the beam rendering at the end of animation
            LineRenderer.enabled = false;
            AnimationFinished.Invoke();
        }

        [Tooltip("Invoked when the beam animation completes.")]
        public UnityEvent AnimationFinished = new();
    }
}