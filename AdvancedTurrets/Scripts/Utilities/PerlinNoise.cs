using UnityEngine;

namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// Generates Perlin noise values with a random offset for variation.
    /// </summary>
    public class PerlinNoise
    {
        private Vector2 offset;

        public PerlinNoise()
        {
            offset.x = Random.Range(-32f, 32f);
            offset.y = Random.Range(-32f, 32f);
        }

        public float GetNoise(float time, float amplitude, float frequency)
        {
            var timeFrequency = time * frequency;
            var perlinNoise = Mathf.PerlinNoise(timeFrequency + offset.x, timeFrequency + offset.y) - 0.5f;
            return perlinNoise * amplitude;
        }
    }
}