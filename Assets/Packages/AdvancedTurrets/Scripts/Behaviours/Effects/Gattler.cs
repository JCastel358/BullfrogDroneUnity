using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Behaviours.Effects
{
    /// <summary>
    /// Simulates a gatling-style rotation with configurable acceleration and deceleration mechanics.
    /// Gradually increases rotation speed when active and automatically decelerates after a set duration of inactivity.
    /// </summary>
    public class Gattler : MonoBehaviour
    {
        [Tooltip("Maximum rotational speed (degrees/sec) when fully spooled up."), Min(0)]
        public float MaxDegSec = 1440;

        [Tooltip("If true, rotates clockwise; otherwise, counterclockwise.")]
        public bool Clockwise = true;

        [Tooltip("Angular acceleration (degrees/sec²) while spooling up."), Min(0)]
        public float SpoolUpAcceleration = 1600;

        [Tooltip("Angular deceleration (degrees/sec²) while spooling down."), Min(0)]
        public float SpoolDownAcceleration = 800;

        [Tooltip("Time after which rotation starts decelerating if inactive."), Min(0)]
        public float SpoolDownAfter = 0.25f;

        private float _angularVelocity;
        private float _lastGattle = float.MinValue;

        private void Update()
        {
            var deltaTime = AdvancedTime.SmartDeltaTime;

            if (AdvancedTime.Time - _lastGattle >= SpoolDownAfter)
            {
                SpoolDown(deltaTime);
            }
            else
            {
                SpoolUp(deltaTime);
            }

            // Clamp angular velocity to max speed
            var absDegSec = Mathf.Abs(MaxDegSec);
            _angularVelocity = Mathf.Clamp(_angularVelocity, -absDegSec, absDegSec);

            // Apply rotation based on current angular velocity
            transform.Rotate(Vector3.forward, _angularVelocity * deltaTime, Space.Self);
        }

        void SpoolUp(float deltaTime)
        {
            var targetVelocity = Clockwise ? -MaxDegSec : MaxDegSec;
            var err = targetVelocity - _angularVelocity;
            if (err != 0)
            {
                var vDt = Mathf.Sign(err) * SpoolUpAcceleration * deltaTime;
                _angularVelocity += vDt;
            }
        }

        void SpoolDown(float deltaTime)
        {
            // Apply spool-down acceleration when inactive
            var err = -_angularVelocity;
            if (err != 0)
            {
                var vDt = Mathf.Sign(err) * SpoolDownAcceleration * deltaTime;
                _angularVelocity += vDt;
            }
        }

        /// <summary>
        /// Refreshes rotation duration maintaining or increasing angular velocity.
        /// </summary>
        public void Gattle()
        {
            _lastGattle = AdvancedTime.Time;
        }
    }
}
