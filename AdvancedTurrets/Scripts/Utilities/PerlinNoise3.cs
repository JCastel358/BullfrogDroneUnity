using UnityEngine;

namespace AdvancedTurrets.Utilities
{
    /// <summary>
    /// Generates Perlin noise as a Vector3
    /// </summary>
    public class PerlinNoise3
    {
        private PerlinNoise _x = new();
        private PerlinNoise _y = new();
        private PerlinNoise _z = new();

        public Vector3 GetNoise(float time, float amplitude, float frequency)
        {
            return new(
                _x.GetNoise(time, amplitude, frequency),
                _y.GetNoise(time, amplitude, frequency),
                _z.GetNoise(time, amplitude, frequency)
            );
        }
    }
}