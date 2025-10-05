using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Complex = System.Numerics.Complex;
using AdvancedTurrets.Libraries;

namespace AdvancedTurrets.Mathematics
{
    public static class AdvancedMathematics
    {
        /// <summary>
        /// Randomizes a float value within a specified percentage range.
        /// </summary>
        /// <param name="value">The original value.</param>
        /// <param name="profilePercent">The percentage range to randomize the value by.</param>
        /// <returns>The randomized float value.</returns>
        public static float Randomize(this float value, float profilePercent)
        {
            if (profilePercent != 0)
            {
                var cRandom = Mathf.Clamp(profilePercent, 0, 1);
                var timeMin = (1 - cRandom) * value;
                var timeMax = (1 + cRandom) * value;
                return UnityEngine.Random.Range(timeMin, timeMax);
            }

            return value;
        }

        /// <summary>
        /// Floors a float value to the nearest multiple of a given increment.
        /// </summary>
        /// <param name="value">The value to floor.</param>
        /// <param name="increment">The increment to round to.</param>
        /// <returns>The floored float value.</returns>
        public static float ToIncrementalFloor(this float value, float increment)
        {
            return Mathf.Floor(value / increment) * increment;
        }

        /// <summary>
        /// Ceils a float value to the nearest multiple of a given increment.
        /// </summary>
        /// <param name="value">The value to ceil.</param>
        /// <param name="increment">The increment to round to.</param>
        /// <returns>The ceiled float value.</returns>
        public static float ToIncrementalCeil(this float value, float increment)
        {
            return Mathf.Ceil(value / increment) * increment;
        }

        # region Equality

        /// <summary>
        /// Checks if a double value is approximately equal to another double value within a given epsilon tolerance.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>True if the values are approximately equal, false otherwise.</returns>
        public static bool Approximately(this double expected, double actual, double epsilon = 1e-7f)
        {
            var epsilonMagnitude = Math.Max(Math.Max(Math.Abs(expected), Math.Abs(actual)), 1.0f);
            return Math.Abs(expected - actual) <= epsilon * epsilonMagnitude;
        }

        /// <summary>
        /// Checks if a float value is approximately equal to another float value within a given epsilon tolerance.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>True if the values are approximately equal, false otherwise.</returns>
        public static bool Approximately(this float expected, float actual, float epsilon = 1e-5f)
        {
            var epsilonMagnitude = Math.Max(Math.Max(Math.Abs(expected), Math.Abs(actual)), 1.0f);
            return Math.Abs(expected - actual) <= epsilon * epsilonMagnitude;
        }

        /// <summary>
        /// Checks if a Vector3 is approximately equal to another Vector3 within a given epsilon tolerance.
        /// </summary>
        /// <param name="expected">The expected vector.</param>
        /// <param name="actual">The actual vector.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>True if the vectors are approximately equal, false otherwise.</returns>
        public static bool Approximately(this Vector3 expected, Vector3 actual, float epsilon = 1e-5f)
        {
            if (expected.x.Approximately(actual.x, epsilon))
            {
                if (expected.y.Approximately(actual.y, epsilon))
                {
                    return expected.z.Approximately(actual.z, epsilon);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a float value is less than or equal to a specified value within a given epsilon tolerance.
        /// </summary>
        /// <param name="val">The value to check.</param>
        /// <param name="lessThanOrEqual">The value to compare to.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>True if the value is less than or equal to the specified value, false otherwise.</returns>
        public static bool LessThanOrEqual(this float val, float lessThanOrEqual, float epsilon = 1e-5f)
        {
            if (val > lessThanOrEqual)
            {
                return val.Approximately(lessThanOrEqual, epsilon);
            }

            return true;
        }

        /// <summary>
        /// Checks if a float value is greater than or equal to a specified value within a given epsilon tolerance.
        /// </summary>
        /// <param name="val">The value to check.</param>
        /// <param name="greaterThanOrEqual">The value to compare to.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>True if the value is greater than or equal to the specified value, false otherwise.</returns>
        public static bool GreaterThanOrEqual(this float val, float greaterThanOrEqual, float epsilon = 1e-5f)
        {
            if (val < greaterThanOrEqual)
            {
                return val.Approximately(greaterThanOrEqual, epsilon);
            }

            return true;
        }

        # endregion

        # region Polynomials

        /// <summary>
        /// Solves a quadratic equation of the form ax^2 + bx + c = 0.
        /// </summary>
        /// <param name="a">The coefficient of the x^2 term.</param>
        /// <param name="b">The coefficient of the x term.</param>
        /// <param name="c">The constant term.</param>
        /// <returns>
        /// An array of <see cref="Complex"/> numbers representing the roots of the quadratic equation.
        /// If the equation has real roots, they will be returned with an imaginary part of 0.
        /// If the equation has complex roots, the real and imaginary parts will be properly set.
        /// </returns>
        /// <remarks>
        /// If the coefficient 'a' is 0, the equation is treated as a linear equation (bx + c = 0), and a single root is returned.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the equation is invalid, such as when the discriminant is negative for real roots.
        /// </exception>
        public static Complex[] Quadratic(double a, double b, double c)
        {
            if (a.Approximately(0))
            {
                var root = -c / b;

                return new Complex[] {
                    // Real
                    new(root, 0)
                };
            }
            else
            {
                var discriminate = b * b - 4 * a * c;

                if (discriminate >= 0)
                {
                    var sqrt = Math.Sqrt(discriminate);
                    var denominator = 2 * a;

                    return new Complex[]
                    {
                        // Real
                        new((-b + sqrt) / denominator, 0),
                        new((-b - sqrt) / denominator, 0)
                    };
                }
                else
                {
                    var complexSqrt = Complex.Sqrt(new(discriminate, 0));
                    var complexDenominator = new Complex(2 * a, 0);

                    return new Complex[]
                    {
                        // Imaginary
                        new(-b + complexSqrt.Real, complexSqrt.Imaginary),
                        new(-b - complexSqrt.Real, -complexSqrt.Imaginary)
                    }.Select(c => c / complexDenominator).ToArray();
                }
            }
        }

        /// <summary>
        /// Solves a cubic equation of the form ax^3 + bx^2 + cx + d = 0.
        /// </summary>
        /// <param name="a">The coefficient of the x^3 term.</param>
        /// <param name="b">The coefficient of the x^2 term.</param>
        /// <param name="c">The coefficient of the x term.</param>
        /// <param name="d">The constant term.</param>
        /// <returns>
        /// An array of <see cref="Complex"/> representing the roots of the cubic equation.
        /// If the equation has complex roots, they will be returned as complex numbers.
        /// </returns>
        public static Complex[] Cubic(double a, double b, double c, double d)
        {
            if (a.Approximately(0))
            {
                return Quadratic(b, c, d);
            }
            else
            {
                var q = (3 * a * c - b * b) / (3 * a * a);
                var r = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);
                var discriminate = r * r / 4 + q * q * q / 27;

                if (discriminate > 0)
                {
                    //1 real root
                    var p = -r / 2d + Math.Sqrt(discriminate);
                    var s = Math.Cbrt(p);
                    var t = -r / 2d - Math.Sqrt(discriminate);
                    var u = Math.Cbrt(t);

                    return new Complex[]
                    {
                         // Real
                         new(s + u - b / (3d * a), 0),
                         
                         // Imaginary
                         new(-(s + u) / 2d - b / (3d * a), (s - u) * Math.Sqrt(3d) / 2d),
                         new(-(s + u) / 2d - b / (3d * a), -(s - u) * Math.Sqrt(3d) / 2d)
                    };
                }
                else if (discriminate == 0)
                {
                    //2 real roots (one is duplicated)
                    var crt = Math.Cbrt(-r / 2);
                    var pt2 = -b / (3 * a);

                    return new Complex[]
                    {
                        // Real
                        new(2 * crt + pt2, 0),
                        new(-crt + pt2, 0),
                    };
                }
                else
                {
                    //3 real roots
                    var i = Math.Sqrt(r * r / 4d - discriminate);
                    var j = Math.Cbrt(i);
                    var k = Math.Acos(-r / (2d * i));
                    var l = -j;
                    var m = Math.Cos(k / 3d);
                    var n = Math.Sqrt(3) * Math.Sin(k / 3d);
                    var p = -b / (3 * a);

                    return new Complex[]
                    {
                        // Real
                        new(2d * j * Math.Cos(k / 3d) - b / (3d * a), 0),
                        new(l * (m + n) + p, 0),
                        new(l * (m - n) + p, 0),
                     };
                }
            }
        }

        /// <summary>
        /// Solves a quartic equation of the form ax^4 + bx^3 + cx^2 + dx + e = 0.
        /// </summary>
        /// <param name="a">The coefficient of the x^4 term.</param>
        /// <param name="b">The coefficient of the x^3 term.</param>
        /// <param name="c">The coefficient of the x^2 term.</param>
        /// <param name="d">The coefficient of the x term.</param>
        /// <param name="e">The constant term.</param>
        /// <returns>
        /// An array of <see cref="Complex"/> representing the roots of the quartic equation.
        /// If the equation has complex roots, they will be returned as complex numbers.
        /// </returns>
        public static Complex[] Quartic(double a, double b, double c, double d, double e)
        {
            if (a.Approximately(0))
            {
                return Cubic(b, c, d, e);
            }
            else
            {
                //Depress the quartic.
                e /= a;
                d /= a;
                c /= a;
                b /= a;

                //Monic quartic. Use a resolvent cubic to solve.
                var b3 = 1;
                var b2 = -c;
                var b1 = d * b - 4 * e;
                var b0 = 4 * e * c - d * d - e * b * b;

                var result = Cubic(b3, b2, b1, b0);

                foreach (var root in result)
                {
                    if (root.Imaginary == 0)
                    {
                        var q_disc = root.Real * root.Real / 4 - e;
                        if (q_disc >= 0)
                        {
                            var p_disc = b * b / 4 + root.Real - c;
                            if (p_disc >= 0)
                            {
                                var pSqrt = Math.Sqrt(p_disc);
                                var bd2 = b / 2;
                                var p1 = bd2 + pSqrt;
                                var p2 = bd2 - pSqrt;

                                var qSqrt = Math.Sqrt(q_disc);
                                var ud2 = root.Real / 2;
                                var q1 = ud2 + qSqrt;
                                var q2 = ud2 - qSqrt;

                                var sanityCheck = (p1 * q2 + p2 * q1).Approximately(d);
                                var quadResult = new List<Complex>();
                                quadResult.AddRange(Quadratic(1, p1, sanityCheck ? q1 : q2));
                                quadResult.AddRange(Quadratic(1, p2, sanityCheck ? q2 : q1));
                                return quadResult.ToArray();
                            }
                        }
                    }
                }

                Debug.LogWarning($"Unable to solve this quartic :( if you can reproduce this please send me the input variables so I can look into a fix. a={a}, b={b}, c={c}, d={d}, e={e}");
                return new Complex[] { default };
            }
        }

        # endregion

        # region Quaternions & Rotations

        /// <summary>
        /// Computes the rotation required to align a point with a target around a specified axis.
        /// </summary>
        /// <remarks>
        /// This method determines the angular displacement needed to rotate an object around a given axis
        /// so that it aligns a specific point with the target position. It calculates projection planes,
        /// rotation angles, and constrained rotations to achieve this effect.
        /// </remarks>
        /// <param name="rotationOrigin">The origin point about which rotation occurs.</param>
        /// <param name="startRotation">The initial rotation before any transformation.</param>
        /// <param name="rotationAxis">The axis around which the rotation takes place in world space.</param>
        /// <param name="point">The point that is being rotated in world space.</param>
        /// <param name="pointForward">The forward direction of the point in world space.</param>
        /// <param name="targetPoint">The target position to which the point is being rotated in world space.</param>
        /// <returns>
        public static DeltaRotation RotateAroundAxis(
            Vector3 rotationOrigin,
            Quaternion startRotation,
            Vector3 rotationAxis,
            Vector3 point,
            Vector3 pointForward,
            Vector3 targetPoint)
        {
            // Compute the orthogonal projection plane of the point onto the forward vector
            var orthogonalPlane = new Plane(pointForward, rotationOrigin);
            var pointDirection = pointForward * Mathf.Sign(Vector3.Dot(pointForward, rotationOrigin - point));
            var pointRay = new Ray(point, pointDirection);
            var orthogonalPoint = orthogonalPlane.GetIntersectPoint(pointRay).GetValueOrDefault(rotationOrigin);

            // Define the rotation plane based on the rotation origin and axis
            var rotationPlane = new Plane(rotationAxis, rotationOrigin);
            var planarOrthogonalPoint = rotationPlane.ClosestPointOnPlane(orthogonalPoint);

            // Project the target onto the rotation plane
            var planarTarget = rotationPlane.ClosestPointOnPlane(targetPoint);
            var normalizedPlanarPointDirection = rotationPlane.GetProjectedDirectionOnPlane(pointForward).normalized;
            var r = Vector3.Distance(planarOrthogonalPoint, rotationOrigin);

            // Compute the rotation angle theta
            var h = (planarTarget - rotationOrigin).magnitude;
            var theta = 0f;
            if (h >= r)
            {
                var c = h * Mathf.Sin(Mathf.PI / 2 - Mathf.Asin(r / h));
                var dir1 = planarOrthogonalPoint + normalizedPlanarPointDirection * c - rotationOrigin;
                var dir2 = planarTarget - rotationOrigin;
                theta = Vector3.SignedAngle(dir1, dir2, rotationAxis);
            }

            // Compute the rotation quaternion based on theta
            var deltaRotationAngleAxis = Quaternion.AngleAxis(theta, rotationAxis);
            var deltaRotation = DeltaRotation.FromDelta(rotationOrigin, startRotation, rotationOrigin, deltaRotationAngleAxis);

            // Return the computed axial rotation
            return deltaRotation;
        }

        /// <summary>
        /// Computes the intersection point of a ray with a plane, if it exists.
        /// </summary>
        /// <param name="plane">The plane to check for intersection.</param>
        /// <param name="ray">The ray to intersect with the plane.</param>
        /// <returns>The intersection point if the ray intersects the plane; otherwise, <c>null</c>.</returns>
        public static Vector3? GetIntersectPoint(this Plane plane, Ray ray)
        {
            if (plane.Raycast(ray, out var d))
            {
                return ray.origin + ray.direction * d;
            }

            return default;
        }

        /// <summary>
        /// Projects a given direction onto the specified plane.
        /// </summary>
        /// <param name="plane">The plane onto which the direction is projected.</param>
        /// <param name="direction">The direction to project.</param>
        /// <returns>The direction projected onto the plane.</returns>
        public static Vector3 GetProjectedDirectionOnPlane(this Plane plane, Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, plane.normal);
        }

        /// <summary>
        /// Transforms a direction vector by a quaternion rotation.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to apply to the direction.</param>
        /// <param name="direction">The direction vector to be transformed.</param>
        /// <returns>The transformed direction vector.</returns>
        public static Vector3 TransformDirection(this Quaternion rotation, Vector3 direction)
        {
            var m4x4 = Matrix4x4.Rotate(rotation);
            return TransformDirection(m4x4, direction);
        }

        /// <summary>
        /// Transforms a direction vector by a matrix transformation.
        /// </summary>
        /// <param name="matrix4X4">The transformation matrix to apply to the direction.</param>
        /// <param name="direction">The direction vector to be transformed.</param>
        /// <returns>The transformed direction vector.</returns>
        public static Vector3 TransformDirection(this Matrix4x4 matrix4X4, Vector3 direction)
        {
            return matrix4X4.MultiplyVector(direction);
        }

        /// <summary>
        /// Inversely transforms a direction vector by a quaternion rotation.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to inverse transform the direction.</param>
        /// <param name="direction">The direction vector to be inversely transformed.</param>
        /// <returns>The inversely transformed direction vector.</returns>
        public static Vector3 InverseTransformDirection(this Quaternion rotation, Vector3 direction)
        {
            var m4x4 = Matrix4x4.Rotate(rotation);
            return InverseTransformDirection(m4x4, direction);
        }

        /// <summary>
        /// Inversely transforms a direction vector by a matrix transformation.
        /// </summary>
        /// <param name="matrix4X4">The inverse transformation matrix to apply to the direction.</param>
        /// <param name="direction">The direction vector to be inversely transformed.</param>
        /// <returns>The inversely transformed direction vector.</returns>
        public static Vector3 InverseTransformDirection(this Matrix4x4 matrix4X4, Vector3 direction)
        {
            return matrix4X4.inverse.MultiplyVector(direction);
        }

        /// <summary>
        /// Transforms a point in 3D space by a quaternion rotation and a position.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to apply to the point.</param>
        /// <param name="position">The position to apply the transformation to.</param>
        /// <param name="point">The point in space to be transformed.</param>
        /// <param name="scale">Optional scale to apply to the transformation (defaults to Vector3.one if not provided).</param>
        /// <returns>The transformed point in 3D space.</returns>
        public static Vector3 TransformPoint(this Quaternion rotation, Vector3 position, Vector3 point, Vector3? scale = default)
        {
            var m4x4 = Matrix4x4.TRS(position, rotation, scale ?? Vector3.one);
            return TransformPoint(m4x4, point);
        }

        /// <summary>
        /// Transforms a point in 3D space by a matrix transformation.
        /// </summary>
        /// <param name="matrix4X4">The transformation matrix to apply to the point.</param>
        /// <param name="point">The point to be transformed.</param>
        /// <returns>The transformed point in 3D space.</returns>
        public static Vector3 TransformPoint(this Matrix4x4 matrix4X4, Vector3 point)
        {
            return matrix4X4.MultiplyPoint3x4(point);
        }

        /// <summary>
        /// Inversely transforms a point in 3D space by a quaternion rotation and position.
        /// </summary>
        /// <param name="rotation">The quaternion rotation to inverse transform the point.</param>
        /// <param name="position">The position to inverse transform the point.</param>
        /// <param name="point">The point to be inversely transformed.</param>
        /// <param name="scale">Optional scale to apply to the inverse transformation (defaults to Vector3.one if not provided).</param>
        /// <returns>The inversely transformed point in 3D space.</returns>
        public static Vector3 InverseTransformPoint(this Quaternion rotation, Vector3 position, Vector3 point, Vector3? scale = default)
        {
            var m4x4 = Matrix4x4.TRS(position, rotation, scale ?? Vector3.one);
            return InverseTransformPoint(m4x4, point);
        }

        /// <summary>
        /// Inversely transforms a point in 3D space by a matrix transformation.
        /// </summary>
        /// <param name="matrix4X4">The inverse transformation matrix to apply to the point.</param>
        /// <param name="point">The point to be inversely transformed.</param>
        /// <returns>The inversely transformed point in 3D space.</returns>
        public static Vector3 InverseTransformPoint(this Matrix4x4 matrix4X4, Vector3 point)
        {
            return matrix4X4.inverse.MultiplyPoint3x4(point);
        }

        #endregion

        # region Vectors

        /// <summary>
        /// Computes the average Vector of a list of Vector3 values.
        /// </summary>
        public static Vector3 Average(this List<Vector3> vector3s)
        {
            if (!(vector3s?.Count > 0)) return default;
            var x = vector3s.Average(v => v.x);
            var y = vector3s.Average(v => v.y);
            var z = vector3s.Average(v => v.z);
            return new(x, y, z);
        }


        #endregion

        #region Optimality
        public enum ScoreModes
        {
            SmallIsBetter,
            BigIsBetter
        }

        public class RankResult<T>
        {
            public T Key;
            public RankResult(T key, float score)
            {
                Key = key;
                Score = score;
            }

            public float Score { get; set; }
            public float Rank { get; set; }
        }

        /// <summary>
        /// Ranks a collection of elements based on a given scoring function.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="iEnumerable">The collection of elements to rank.</param>
        /// <param name="score">A function that computes a score for each element.</param>
        /// <param name="rankWeight">The weight applied to rank values (default: 1).</param>
        /// <param name="scoreMode">Determines whether smaller or larger scores are better (default: SmallIsBetter).</param>
        public static IEnumerable<RankResult<T>> RankBy<T>(this IEnumerable<T> iEnumerable, System.Func<T, float> score, float rankWeight = 1, ScoreModes scoreMode = ScoreModes.SmallIsBetter)
        {
            // Apply scores to all items and order them accordingly
            var rankResults = iEnumerable
                .Select(t => new RankResult<T>(t, score.Invoke(t)))
                .OrderBy(a => scoreMode == ScoreModes.SmallIsBetter ? a.Score : -a.Score);

            // Assign ranks based upon sort order
            rankResults.AssignRanks(rankWeight);
            return rankResults;
        }

        /// <summary>
        /// Re-ranks a collection of already ranked elements based on a new scoring function.
        /// </summary>
        /// <typeparam name="V">The type of elements in the collection.</typeparam>
        /// <param name="rankResults">A collection of ranked results.</param>
        /// <param name="score">A function that computes a new score for each ranked element.</param>
        /// <param name="rankWeight">The weight applied to rank values (default: 1).</param>
        /// <param name="scoreMode">Determines whether smaller or larger scores are better (default: SmallIsBetter).</param>
        public static IEnumerable<RankResult<V>> RankBy<V>(this IEnumerable<RankResult<V>> rankResults, System.Func<V, float> score, float rankWeight = 1, ScoreModes scoreMode = ScoreModes.SmallIsBetter)
        {
            // Apply scores to all items and order them accordingly
            var result = rankResults
                .ForEach(rr => rr.Score = score.Invoke(rr.Key))
                .OrderBy(a => scoreMode == ScoreModes.SmallIsBetter ? a.Score : -a.Score);

            // Assign ranks based upon sort order
            result.AssignRanks(rankWeight);
            return rankResults;
        }

        private static void AssignRanks<T>(this IEnumerable<RankResult<T>> rankResults, float weight)
        {
            var rank = 1;
            var lastScore = default(float?);
            rankResults.ForEach(rr =>
            {
                rr.Rank += rank * weight;

                if (lastScore != rr.Score)
                {
                    rank += 1;
                    lastScore = rr.Score;
                }
            });
        }

        /// <summary>
        /// Retrieves the best-ranked item from a collection of ranked results.
        /// </summary>
        /// <typeparam name="T">The type of elements being ranked.</typeparam>
        /// <param name="rankResults">The collection of ranked results.</param>
        public static RankResult<T> Best<T>(this IEnumerable<RankResult<T>> rankResults)
        {
            return rankResults.OrderBy(rr => rr.Rank).First();
        }

        /// <summary>
        /// Retrieves the best-ranked item or returns the default value if the collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of elements being ranked.</typeparam>
        /// <param name="rankResults">The collection of ranked results.</param>
        public static RankResult<T> BestOrDefault<T>(this IEnumerable<RankResult<T>> rankResults)
        {
            if (rankResults.OrderBy(rr => rr.Rank).FirstOrDefault() is RankResult<T> result)
            {
                return result;
            }

            return default;
        }

        # endregion
    }
}