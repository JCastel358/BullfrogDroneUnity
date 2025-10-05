using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace AdvancedTurrets.Mathematics
{
    /// <summary>
    /// Represents a rotation change (delta) between two rotations, and provides methods to transform points, directions, and rotations between them.
    /// </summary>
    public class DeltaRotation
    {
        private Matrix4x4 _fromMatrix;
        private Matrix4x4 _toMatrix;

        public Quaternion Delta;

        private DeltaRotation(Vector3 fromOrigin, Quaternion fromRotation, Vector3 toOrigin, Quaternion delta, Quaternion toRotation)
        {
            _fromMatrix = Matrix4x4.TRS(fromOrigin, fromRotation, Vector3.one);
            _toMatrix = Matrix4x4.TRS(toOrigin, toRotation, Vector3.one);
            Delta = delta;
        }

        public Vector3 FromPosition => _fromMatrix.GetPosition();

        public Vector3 ToPosition => _toMatrix.GetPosition();

        public Quaternion FromRotation => _fromMatrix.rotation;

        public Quaternion ToRotation => _toMatrix.rotation;

        /// <summary>
        /// Creates a <see cref="DeltaRotation"/> from a given initial rotation and a delta rotation.
        /// </summary>
        /// <param name="fromOrigin">The origin of the initial rotation.</param>
        /// <param name="fromRotation">The initial rotation.</param>
        /// <param name="toOrigin">The origin of the target rotation.</param>
        /// <param name="deltaRotation">The rotation change (delta) applied to the initial rotation.</param>
        public static DeltaRotation FromDelta(Vector3 fromOrigin, Quaternion fromRotation, Vector3 toOrigin, Quaternion deltaRotation)
        {
            var toRotation = deltaRotation * fromRotation;
            return new DeltaRotation(fromOrigin, fromRotation, toOrigin, deltaRotation, toRotation);
        }

        /// <summary>
        /// Creates a <see cref="DeltaRotation"/> from a given initial rotation and the target rotation.
        /// </summary>
        /// <param name="fromOrigin">The origin of the initial rotation.</param>
        /// <param name="fromRotation">The initial rotation.</param>
        /// <param name="toOrigin">The origin of the target rotation.</param>
        /// <param name="toRotation">The target rotation.</param>
        public static DeltaRotation FromTo(Vector3 fromOrigin, Quaternion fromRotation, Vector3 toOrigin, Quaternion toRotation)
        {
            var deltaRotation = toRotation * Quaternion.Inverse(fromRotation);
            return new DeltaRotation(fromOrigin, fromRotation, toOrigin, deltaRotation, toRotation);
        }

        public Vector3 TransformPoint(Vector3 startPoint)
        {
            return _toMatrix.TransformPoint(_fromMatrix.InverseTransformPoint(startPoint));
        }

        public Vector3 InverseTransformPoint(Vector3 endPoint)
        {
            return _fromMatrix.TransformPoint(_toMatrix.InverseTransformPoint(endPoint));
        }

        public Vector3 TransformDirection(Vector3 startDirection)
        {
            return _toMatrix.TransformDirection(_fromMatrix.InverseTransformDirection(startDirection));
        }

        public Vector3 InverseTransformDirection(Vector3 endDirection)
        {
            return _fromMatrix.TransformDirection(_toMatrix.InverseTransformDirection(endDirection));
        }

        public Quaternion TransformRotation(Quaternion startRotation)
        {
            return Delta * startRotation;
        }
    }
}