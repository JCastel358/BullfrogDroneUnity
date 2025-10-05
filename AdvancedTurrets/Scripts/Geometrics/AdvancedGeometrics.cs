using System.Collections.Generic;
using System.Linq;
using AdvancedTurrets.Libraries;
using UnityEngine;

namespace AdvancedTurrets.Geometrics
{
    public static class AdvancedGeometrics
    {
        /// <summary>
        /// Converts a sequence of Vector3 points into a collection of line segments.
        /// </summary>
        public static IEnumerable<Line> AsLines(this IEnumerable<Vector3> points, bool connectEndpoints = false)
        {
            var from = default(Vector3?);
            foreach (var point in points)
            {
                if (from.HasValue)
                {
                    yield return new Line(from.Value, point);
                }
                from = point;
            }

            if (connectEndpoints && from.HasValue)
            {
                yield return new(from.Value, points.First());
            }
        }

        /// <summary>
        /// Converts a sequence of Lines into a sequence of points.
        /// </summary>
        public static IEnumerable<Vector3> AsPoints(this IEnumerable<Line> lines, bool removeDuplicates = true)
        {
            var points = new List<Vector3>();
            foreach (var line in lines)
            {
                points.Add(line.From);
                points.Add(line.To);
            }

            if (removeDuplicates)
            {
                return points.RemoveDuplicates().ToList();
            }

            return points;
        }
    }
}
