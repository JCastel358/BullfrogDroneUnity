
using UnityEngine;

namespace AdvancedTurrets.Visualization
{
    public static class AdvancedColors
    {
        public static readonly Color TransparentWhite = GetUnscaledRGBA(255, 255, 255, 75);
        public static readonly Color LightGray = GetUnscaledRGBA(100, 100, 100, 75);
        public static readonly Color DarkGray = GetUnscaledRGBA(50, 50, 50, 75);
        public static readonly Color LightGreen = GetUnscaledRGBA(0, 255, 0, 75);
        public static readonly Color DarkGreen = GetUnscaledRGBA(0, 100, 0, 75);
        public static readonly Color LightRed = GetUnscaledRGBA(255, 0, 0, 75);

        static Color GetUnscaledRGBA(float r255, float g255, float b255, float a255)
        {
            var d = 255f;
            return new(r255 / d, g255 / d, b255 / d, a255 / d);
        }

        /// <summary>
        /// Adjusts a color's hue based on a given percentage.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="percent">The percentage (0 to 1) that affects the hue of the color.</param>
        /// <returns>A modified color with its hue scaled by the given percentage.</returns>
        public static Color GetColorByPercent(Color color, float percent)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);

            var clampedPercent = Mathf.Clamp01(percent);
            var pH = h * clampedPercent;
            return Color.HSVToRGB(pH, 1f, 1f);
        }
    }
}