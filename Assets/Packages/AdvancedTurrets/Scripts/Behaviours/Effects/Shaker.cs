using System;
using System.Collections;
using AdvancedTurrets.Utilities;
using UnityEngine;


namespace AdvancedTurrets.Behaviours.Effects
{
    /// <summary>
    /// Applies a shaking effect to a GameObject using Perlin noise.
    /// </summary>
    public class Shaker : MonoBehaviour
    {
        private Coroutine _coroutine;

        /// <summary>
        /// Initiates a shake effect with the specified magnitude.
        /// If a shake is already in progress, it stops the previous one.
        /// </summary>
        public void Shake(float magnitude)
        {
            if (_coroutine != default)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(AnimateNoise(magnitude));
        }

        /// <summary>
        /// Smoothly interpolates a float value over time and invokes a callback with the updated value.
        /// </summary>
        public static IEnumerator InterpolateFloat(float current, float target, float duration, Action<float> onInterpolate)
        {
            var t0 = AdvancedTime.Time;
            var initial = current;
            var delta = target - initial;

            while (true)
            {
                var timeStep = (Time.time - t0) / duration;
                var value = initial + delta * timeStep;
                onInterpolate(value);

                if ((Time.time - t0) >= duration)
                {
                    onInterpolate(target);
                    break;
                }

                yield return null;
            }
        }
        /// <summary>
        /// Animates the shaking effect by modifying position and rotation using Perlin noise.
        /// </summary>
        private IEnumerator AnimateNoise(float magnitude)
        {
            var positionNoise = new PerlinNoise3();
            var rotationNoise = new PerlinNoise3();

            if (transform.parent)
            {
                var rp0 = transform.localPosition;
                var rr0 = transform.localEulerAngles;
                yield return InterpolateFloat(magnitude, 0, 0.5f, amplitude =>
                {
                    transform.localPosition = rp0 + positionNoise.GetNoise(Time.unscaledTime, amplitude, 3);
                    transform.localEulerAngles = rr0 + rotationNoise.GetNoise(Time.unscaledTime, amplitude, 3);
                });
            }
            else
            {
                var p0 = transform.position;
                var r0 = transform.eulerAngles;
                yield return InterpolateFloat(magnitude, 0, 0.5f, amplitude =>
                {
                    transform.position = p0 + positionNoise.GetNoise(Time.unscaledTime, amplitude, 3);
                    transform.eulerAngles = r0 + rotationNoise.GetNoise(Time.unscaledTime, amplitude, 3);
                });
            }
        }
    }
}