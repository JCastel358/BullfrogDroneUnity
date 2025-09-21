using UnityEngine;

namespace AdvancedTurrets.Kinematics
{
    /// <summary>
    /// A collection of static methods for calculating kinematic positions and velocities based on different time handling techniques.
    /// These methods support various time steps: delta time, fixed time, and instantiated time, each with different behaviors for calculations.
    /// </summary>
    public class AdvancedKinematics
    {
        /// <summary>
        /// Computes the expected position at time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This will use the optimal and appropriate kinematic function from parameters
        /// </remarks>
        /// <param name="t">Time elapsed.</param>
        /// <param name="p">Initial position.</param>
        /// <param name="v">Initial velocity.</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Gravity.</param>
        /// <param name="createdInFixedTime">Indicates if the object was instantiated in fixed time.</param>
        /// <param name="instantiatedOrEnabled">Indicates if the object has been instantiated or enabled.</param>
        /// <returns>Predicted position after <paramref name="t"/> seconds.</returns>
        public static Vector3 GetPosition(float t, Vector3 p, Vector3 v, Vector3 a, Vector3 g, bool createdInFixedTime, bool instantiatedOrEnabled)
        {
            if (!createdInFixedTime)
            {
                return GetDeltaTimePosition1(t, p, v, a, g);
            }
            else if (!instantiatedOrEnabled)
            {
                return GetFixedTimePosition1(t, p, v, a, g);
            }
            else
            {
                return GetFixedTimeInstantiatedPosition1(t, p, v, a, g);
            }
        }

        /// <summary>
        /// Computes the velocity at time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This will use the optimal and appropriate kinematic function from parameters
        /// </remarks>
        /// <param name="t">Time at which to compute the velocity.</param>
        /// <param name="v">Initial velocity.</param>
        /// <param name="a">Acceleration vector.</param>
        /// <param name="g">Gravity vector.</param>
        /// <param name="createdInFixedTime">Indicates if the calculation is based on fixed time.</param>
        /// <param name="instantiatedOrEnabled">Indicates if the object is instantiated or enabled.</param>
        /// <returns>Computed velocity at time <paramref name="t"/>.</returns>
        public static Vector3 GetVelocity(float t, Vector3 v, Vector3 a, Vector3 g, bool createdInFixedTime, bool instantiatedOrEnabled)
        {
            if (!createdInFixedTime)
            {
                return GetDeltaTimeVelocity(t, v, a, g);
            }
            else if (!instantiatedOrEnabled)
            {
                return GetFixedTimeVelocity(t, v, a, g);
            }
            else
            {
                return GetFixedTimeInstantiatedVelocity(t, v, a, g);
            }
        }

        # region DeltaTime

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in DeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in DeltaTime.</param>
        /// <param name="v">Initial velocity as measured in DeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetDeltaTimePosition1(float t, Vector3 p, Vector3 v, Vector3 a, Vector3 g)
        {
            if (t == default)
            {
                return p;
            }

            var t2 = (t * t);
            return p + v * t + 0.5f * t2 * (a + g) + 0.01f * t * (a + g);
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/> using initial and final velocity.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in DeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in DeltaTime.</param>
        /// <param name="v_1">Initial velocity as measured in DeltaTime</param>
        /// <param name="v_2">Final velocity as expected in DeltaTime.</param>
        public static Vector3 GetDeltaTimePosition2(float t, Vector3 p, Vector3 v_1, Vector3 v_2)
        {
            if (t == default)
            {
                return p;
            }

            return p + (t + 0.02f) * 0.5f * v_2 + (t - 0.02f) * 0.5f * v_1;
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in DeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in DeltaTime.</param>
        /// <param name="v_1">Initial velocity as measured in DeltaTime</param>
        /// <param name="v_2">Final velocity as expected in DeltaTime.</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetDeltaTimePosition3(Vector3 p, Vector3 v_1, Vector3 v_2, Vector3 a, Vector3 g)
        {
            static float GetPosition3(float p, float v_1, float v_2, float a, float g)
            {
                var v2_2 = v_2 * v_2;
                var v1_2 = v_1 * v_1;
                return p + 0.5f * (v2_2 - v1_2) / (a + g) + 0.01f * (v_2 - v_1);
            }

            return new(
                GetPosition3(p.x, v_1.x, v_2.x, a.x, g.x),
                GetPosition3(p.y, v_1.y, v_2.y, a.y, g.y),
                GetPosition3(p.z, v_1.z, v_2.z, a.z, g.z));
        }

        /// <summary>
        /// Computes the object's velocity after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in DeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="v">Initial velocity as measured in DeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetDeltaTimeVelocity(float t, Vector3 v, Vector3 a, Vector3 g)
        {
            if (t == default)
            {
                return v;
            }

            return v + (a + g) * t;
        }

        # endregion

        # region FixedTime

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in FixedDeltaTime.</param>
        /// <param name="v">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimePosition1(float t, Vector3 p, Vector3 v, Vector3 a, Vector3 g)
        {
            return p + v * t + (t * t + .02f * t) * 0.5f * (a + g);
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in FixedDeltaTime.</param>
        /// <param name="v_1">Initial velocity.</param>
        /// <param name="v_2">Final velocity.</param>
        public static Vector3 GetFixedTimePosition2(float t, Vector3 p, Vector3 v_1, Vector3 v_2)
        {
            if (t == default)
            {
                return p;
            }

            return p + (t + .02f) * 0.5f * v_2 + (t - .02f) * 0.5f * v_1;
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Initial position as measured in FixedDeltaTime.</param>
        /// <param name="v_1">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="v_2">Final velocity as expected in FixedDeltaTime.</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimePosition3(Vector3 p, Vector3 v_1, Vector3 v_2, Vector3 a, Vector3 g)
        {
            static float GetFixedPosition3(float p, float v_1, float v_2, float a, float g)
            {
                return p + 0.5f * (v_2 * v_2 - v_1 * v_1) / (a + g) + 0.01f * (v_2 - v_1);
            }

            return new(
                GetFixedPosition3(p.x, v_1.x, v_2.x, a.x, g.x),
                GetFixedPosition3(p.y, v_1.y, v_2.y, a.y, g.y),
                GetFixedPosition3(p.z, v_1.z, v_2.z, a.z, g.z));
        }

        /// <summary>
        /// Computes the object's velocity after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="v">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimeVelocity(float t, Vector3 v, Vector3 a, Vector3 g)
        {
            // Note - to maintain API intuition this is here. But FT and DT velocity calculations are the same.
            return GetDeltaTimeVelocity(t, v, a, g);
        }

        # endregion

        # region FixedTime Instantiated

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured at instantiation in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Instantiated position as measured in FixedDeltaTime.</param>
        /// <param name="v">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimeInstantiatedPosition1(float t, Vector3 p, Vector3 v, Vector3 a, Vector3 g)
        {
            var hfdt = .01f;
            var hfdt_2 = hfdt * hfdt;

            var tp = t + hfdt;
            var tp_2 = tp * tp;
            var tm = t - hfdt;
            var tm_2 = tm * tm;
            return p + v * t + 0.5f * tp_2 * g - 0.5f * hfdt_2 * g + 0.5f * tm_2 * a - 0.5f * hfdt_2 * a;
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured at instantiation in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Instantiated position as measured in FixedDeltaTime.</param>
        /// <param name="v_1">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="v_2">Final velocity as expected in FixedDeltaTime.</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimeInstantiatedPosition2(float t, Vector3 p, Vector3 v_1, Vector3 v_2, Vector3 g)
        {
            return p + 0.5f * t * v_1 + 0.5f * t * v_2 + 0.01f * t * g;
        }

        /// <summary>
        /// Computes the object's position after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured at instantiation in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="p">Instantiated position as measured in FixedDeltaTime.</param>
        /// <param name="v_1">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="v_2">Final velocity as expected in FixedDeltaTime.</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimeInstantiatedPosition3(Vector3 p, Vector3 v_1, Vector3 v_2, Vector3 a, Vector3 g)
        {
            static float GetFixedInstantiatedPosition3(float p, float v_1, float v_2, float a, float g)
            {
                var v_1_2 = v_1 * v_1;
                var v_2_2 = v_2 * v_2;
                return p + 0.0002f * (a * (g + 50f * v_2) - 50f * (g * (2 * v_1 - v_2) + 50f * (v_1_2 - v_2_2))) / (a + g) + .01f * v_1;
            }

            return new(
                GetFixedInstantiatedPosition3(p.x, v_1.x, v_2.x, a.x, g.x),
                GetFixedInstantiatedPosition3(p.y, v_1.y, v_2.y, a.y, g.y),
                GetFixedInstantiatedPosition3(p.z, v_1.z, v_2.z, a.z, g.z));
        }

        /// <summary>
        /// Computes the object's velocity after time <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// This equation is only accurate with parameters measured at instantiation in FixedDeltaTime
        /// </remarks>
        /// <param name="t">Elapsed time.</param>
        /// <param name="v">Initial velocity as measured in FixedDeltaTime</param>
        /// <param name="a">Constant acceleration applied over <paramref name="t"/> in FixedUpdate().</param>
        /// <param name="g">Constant physics gravity applied over <paramref name="t"/>. Should be equivalent to Physics.Gravity if applicable.</param>
        public static Vector3 GetFixedTimeInstantiatedVelocity(float t, Vector3 v, Vector3 a, Vector3 g)
        {
            if (t == default)
            {
                return v;
            }

            return v + a * (t - 0.02f) + g * t;
        }

        # endregion
    }

}
