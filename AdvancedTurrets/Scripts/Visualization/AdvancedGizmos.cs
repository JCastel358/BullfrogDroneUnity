using System.Linq;
using AdvancedTurrets.Geometrics;
using UnityEditor;
using UnityEngine;

namespace AdvancedTurrets.Visualization
{
    public static partial class AdvancedGizmos
    {
        /// <summary>
        /// Draws a circle using Unity's Gizmos.
        /// </summary>
        /// <param name="center">The world-space position of the circle's center.</param>
        /// <param name="normal">The normal vector defining the plane in which the circle is drawn.</param>
        /// <param name="up">The up vector used for orientation.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="lines">The number of line segments used to draw the circle.</param>
        /// <param name="degrees">The angular span of the circle in degrees (default is 360 for a full circle).</param>
        /// <param name="clockwise">Determines whether the circle is drawn in a clockwise direction.</param>
        /// <param name="startOffsetDegrees">Offset in degrees from which to start drawing the circle.</param>
        public static void DrawCircle(Vector3 center, Vector3 normal, Vector3 up, float radius, int lines = 45, float degrees = 360f, bool clockwise = true, float startOffsetDegrees = 0f)
        {
            var matrix = Gizmos.matrix;
            var rotation = Quaternion.LookRotation(normal, up);
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);

            var range = Enumerable.Range(0, lines + 1);
            foreach (var line in range.Select(i =>
            {
                if (clockwise) i *= -1;
                var angle = (float)i / lines * degrees + startOffsetDegrees;
                var x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                var z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                return new Vector3(x, z);
            }).AsLines())
            {
                DrawLine(line);
            }

            Gizmos.matrix = matrix;
        }

        /// <summary>
        /// Draws a dotted line between two points in Unity's Gizmos.
        /// </summary>
        /// <param name="from">The starting point of the dotted line.</param>
        /// <param name="to">The ending point of the dotted line.</param>
        /// <param name="dashes">The number of dash segments in the dotted line.</param>
        /// <param name="label">An optional text label displayed at the midpoint of the line.</param>
        public static void DrawDottedLine(Vector3 from, Vector3 to, float dashes = 5, string label = default)
        {
            var distance = Vector3.Distance(from, to);
            var direction = (to - from).normalized;
            var dashLengths = distance / ((dashes * 2) - 1);
            for (float i = 0; i < distance; i += dashLengths * 2)
            {
                var iFrom = from + direction * i;
                var iTo = from + direction * Mathf.Min(i + dashLengths, distance);
                Gizmos.DrawLine(iFrom, iTo);
            }

            if (!string.IsNullOrEmpty(label))
            {
                Handles.Label(from + (to - from) / 2, label, new GUIStyle
                {
                    normal = new GUIStyleState { textColor = Gizmos.color },
                    fontSize = 25,
                });
            }
        }

        /// <summary>
        /// Draws a single line in Unity's Gizmos.
        /// </summary>
        /// <param name="line">The line to be drawn, specified as a <see cref="Line"/> object.</param>
        public static void DrawLine(Line line)
        {
            Gizmos.DrawLine(line.From, line.To);
        }
    }

}
