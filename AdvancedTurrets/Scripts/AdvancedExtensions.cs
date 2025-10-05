
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AdvancedTurrets.Libraries
{
    // todo put extensions where they matter/relate to
    public static class AdvancedExtensions
    {
        /// <summary>
        /// Splits a list into a specified number of sublists of equal length (last list is remainder).
        /// </summary>
        public static List<List<T>> Split<T>(this List<T> list, int count)
        {
            var result = new List<List<T>>();

            var chunkSize = list.Count / count;
            var remainder = list.Count % count;

            var index = 0;
            for (int i = 0; i < count; i++)
            {
                int currentChunkSize = chunkSize + (i < remainder ? 1 : 0);
                result.Add(list.Skip(index).Take(currentChunkSize).ToList());
                index += currentChunkSize;
            }

            return result;
        }

        /// <summary>
        /// Enumerates elements and checks for the predicate. If true, returns True with a pointer to the associated element.
        /// </summary>
        public static bool FindFirst<T>(this IEnumerable<T> elements, Func<T, bool> predicate, out T first)
        {
            foreach (var e in elements)
            {
                if (predicate(e))
                {
                    first = e;
                    return true;
                }
            }

            first = default;
            return false;
        }

        /// <summary>
        /// Generates a sequence of numbers from a starting value to an end value with a specified step size.
        /// </summary
        public static IEnumerable<float> EnumerateInterval(this float from, float to, float step)
        {
            yield return from;
            while (from < to)
            {
                from += step;
                from = Mathf.Min(from, to);
                yield return from;
            }
        }

        /// <summary>
        /// Randomizes the direction of a given vector within a specified spread (degrees).
        /// </summary>
        public static Vector3 RandomizeDirection(this Vector3 direction, float spread)
        {
            if (spread == default)
            {
                return direction;
            }

            return RandomizeDirection(Quaternion.LookRotation(direction), spread) * Vector3.forward * direction.magnitude;
        }

        /// <summary>
        /// Removes dictionary entries that match a given predicate.
        /// </summary>
        public static KeyValuePair<K, V>[] RemoveWhere<K, V>(this Dictionary<K, V> dict, Func<KeyValuePair<K, V>, bool> removeIf)
        {
            var removedKeyValuePairs = dict.Where(kvp => removeIf.Invoke(kvp)).ToArray();
            foreach (var kvp in removedKeyValuePairs)
            {
                dict.Remove(kvp.Key);
            }
            return removedKeyValuePairs;
        }

        /// <summary>
        /// Removes elements from a list that match a given predicate.
        /// </summary>
        public static IEnumerable<T> RemoveWhere<T>(this List<T> tList, Func<T, bool> removeIf)
        {
            for (int i = tList.Count - 1; i >= 0; i--)
            {
                var t = tList[i];
                if (removeIf.Invoke(t))
                {
                    tList.Remove(t);
                }
            }

            return tList;
        }

        /// <summary>
        /// Rotates a quaternion around its X-axis.
        /// </summary>
        public static Quaternion RotateX(this Quaternion direction, float deg)
        {
            if (!Mathf.Approximately(deg, 0))
            {
                direction *= Quaternion.AngleAxis(deg, Vector3.right);
            }

            return direction;
        }

        /// <summary>
        /// Rotates a quaternion around the Y-axis.
        /// </summary>
        public static Quaternion RotateY(this Quaternion direction, float deg)
        {
            if (!Mathf.Approximately(deg, 0))
            {
                direction *= Quaternion.AngleAxis(deg, Vector3.up);
            }

            return direction;
        }

        /// <summary>
        /// Rotates a quaternion around the Z-axis.
        /// </summary>
        public static Quaternion RotateZ(this Quaternion direction, float deg)
        {
            if (!Mathf.Approximately(deg, 0))
            {
                direction *= Quaternion.AngleAxis(deg, Vector3.forward);
            }

            return direction;
        }

        /// <summary>
        /// Applies a random rotation in any direction within a specified spread angle.
        /// </summary>
        public static Quaternion RandomizeDirection(this Quaternion direction, float spread)
        {
            if (spread == default)
            {
                return direction;
            }

            return direction * Quaternion.AngleAxis(Random.Range(-spread, spread), Random.insideUnitSphere);
        }

        /// <summary>
        /// Randomly selects an element from a collection or returns default if the collection is empty.
        /// </summary>
        public static T RandomOrDefault<T>(this IEnumerable<T> elements)
        {
            var size = elements.Count();
            return size > 0 ? elements.ElementAt(Random.Range(0, size)) : default;
        }

        /// <summary>
        /// Prunes duplicate values from the list while maintaining order.
        /// </summary>
        public static List<T> RemoveDuplicates<T>(this IEnumerable<T> elements)
        {
            var hashset = new HashSet<T>();
            return elements.Where(e => hashset.Add(e)).ToList();
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
        {
            foreach (var element in iEnumerable)
            {
                action.Invoke(element);
            }

            return iEnumerable;
        }

        # region Version Compatibility

        /// <summary>
        /// Version compatibility support for rigidbody velocity
        /// </summary>
        public static Vector3 GetVelocity(this Rigidbody rigidbody)
        {
#if UNITY_6000_0_OR_NEWER
            return rigidbody.linearVelocity;
#else
            return rigidbody.velocity;
#endif
        }

        /// <summary>
        /// Version compatibility support for rigidbody velocity
        /// </summary>
        public static void SetVelocity(this Rigidbody rigidbody, Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = velocity;
#else
            rigidbody.velocity = velocity;
#endif
        }

        # endregion
    }
}