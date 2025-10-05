using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedTurrets.Geometrics
{
    /// <summary>
    /// Represents a line segment in 3D space defined by two points: <see cref="From"/> and <see cref="To"/>.
    /// </summary>
    [System.Serializable]
    [Tooltip("Line is represented as a direction and magnitude between two three-dimensional points")]
    public struct Line
    {
        [Tooltip("The point from which the line begins")]
        public Vector3 From;

        [Tooltip("The point at which the line ends")]
        public Vector3 To;

        public Line(Vector3 from, Vector3 to)
        {
            From = from;
            To = to;
        }

        public readonly Vector3 Direction => To - From;

        public readonly float Magnitude => Direction.magnitude;

        /// <summary>
        /// Evaluates the line at a given distance if its within the bounds of the line.
        /// </summary>
        public readonly bool EvaluateAt(float distance, out Vector3 point)
        {
            if (distance > Magnitude || distance < 0)
            {
                point = default;
                return false;
            }

            point = From + Direction.normalized * distance;
            return true;
        }

        # region Physics QOL

        /// <summary>
        /// This is a permanent buffer for all Line related physics operations.
        /// </summary>
        static readonly RaycastHit[] _raycastBuffer = new RaycastHit[100];

        public readonly bool Raycast(LayerMask layerMask, out RaycastHit raycastHit)
        {
            return Physics.Raycast(From, Direction.normalized, out raycastHit, Magnitude, layerMask);
        }

        public readonly IEnumerable<RaycastHit> RaycastAll(LayerMask layerMask)
        {
            var count = Physics.RaycastNonAlloc(From, Direction.normalized, _raycastBuffer, Magnitude, layerMask);
            return _raycastBuffer.Take(count);
        }

        public readonly IEnumerable<RaycastHit> SphereCastAll(float radius, LayerMask layerMask)
        {
            var count = Physics.SphereCastNonAlloc(From, radius, Direction.normalized, _raycastBuffer, Magnitude, layerMask);
            return _raycastBuffer.Take(count);
        }

        # endregion
    }
}
