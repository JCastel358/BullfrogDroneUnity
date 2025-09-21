using System;
using System.Linq;
using AdvancedTurrets.Behaviours.Lifecycles;
using UnityEditor;
using UnityEngine;


namespace AdvancedTurrets.Visualization
{
    [Serializable]
    public class AdvancedHealthGizmos : BaseGizmos
    {
        [Tooltip("Scale factor for the health bar.")]
        public float Scalar = 1;

        [Tooltip("Whether to change color to red when health is depleted.")]
        public bool DepleteColorToRed = true;

        [Tooltip("The alpha modifier of the bar when it is unavailable for taking damage")]
        public float UnavailableAlphaFactor = 0.25f;

        [Tooltip("Size of the health bar.")]
        public Vector2 BarSize = new(500, 50);

        [Tooltip("Offset position for the health bar.")]
        public Vector3 OffsetPosition = new(0, 0);

        [Tooltip("Font size of the health display.")]
        public int FontSize = 11;

        [Tooltip("The name to display on the health bar.")]
        public string DisplayName = "Health";

        [Tooltip("Whether to display the name of the health bar.")]
        public bool ShowName = false;

        static Vector3[] GetBarVerts(Vector3 center, Vector3 size)
        {
            var halfSize = size / 2;
            var ur = center + new Vector3(halfSize.x, halfSize.y, 0);  // Upper-right corner
            var lr = center + new Vector3(halfSize.x, -halfSize.y, 0); // Lower-right corner
            var ll = center + new Vector3(-halfSize.x, -halfSize.y, 0); // Lower-left corner
            var ul = center + new Vector3(-halfSize.x, halfSize.y, 0); // Upper-left corner
            return new[] { ur, lr, ll, ul };  // Return the array of vertices
        }

        static Vector3[] GetFillVerts(Vector3 center, Vector3 size, float fillPercent)
        {
            var ul = -size.x / 2; // Upper-left X position
            var ur = size.x / 2;  // Upper-right X position
            var fillCenterX = ul + (ur - ul) / 2 * fillPercent; // Calculate the center X position based on the fill percentage
            var fillCenter = center + new Vector3(fillCenterX, 0); // Calculate the filled center position
            var fillSize = new Vector3(size.x * fillPercent, size.y); // Scale the size based on the fill percentage
            return GetBarVerts(fillCenter, fillSize); // Return the filled rectangle's vertices
        }

        static void DrawHealthbar(AdvancedHealth advancedHealth, Vector3 shieldBarCenter, Vector3 barSize, Color color, float unavailableFactor, Camera camera)
        {
            var shieldBar = GetBarVerts(shieldBarCenter, barSize);
            var shieldBackgroundVerts = shieldBar.Select(sp => camera.ScreenToWorldPoint(sp));
            Handles.DrawSolidRectangleWithOutline(shieldBackgroundVerts.ToArray(), Color.black, color);

            var shieldFillVerts = GetFillVerts(shieldBarCenter, barSize, advancedHealth.Percent).Select(sp => camera.ScreenToWorldPoint(sp));
            if (!advancedHealth.AvailableForDamage)
            {
                color.a *= unavailableFactor;
            }
            Handles.DrawSolidRectangleWithOutline(shieldFillVerts.ToArray(), color, Color.black);
        }

        public void DrawGizmos(AdvancedHealth advancedHealth)
        {
            if (!Enabled || advancedHealth == default)
            {
                return;
            }

            var camera = Camera.current;

            var position = advancedHealth.transform.position;
            var scalar = 1 / HandleUtility.GetHandleSize(position);
            var screenPoint = camera.WorldToScreenPoint(position + OffsetPosition);
            var barSize = Scalar * scalar * BarSize;
            var healthBarCenter = screenPoint;

            var color = DepleteColorToRed ? AdvancedColors.GetColorByPercent(MainColor, advancedHealth.Percent) : MainColor;
            DrawHealthbar(advancedHealth, healthBarCenter, barSize, color, UnavailableAlphaFactor, camera);

            if (ShowName)
            {
                Handles.Label(camera.ScreenToWorldPoint(healthBarCenter + barSize.x / 2 * 1.05f * Vector3.left), DisplayName, new GUIStyle()
                {
                    alignment = TextAnchor.MiddleRight,
                    normal = new()
                    {
                        textColor = Color.white,
                    },
                    fontSize = FontSize,
                });
            }
        }
    }
}