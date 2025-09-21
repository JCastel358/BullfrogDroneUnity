using UnityEngine;


namespace AdvancedTurrets.Visualization
{
    /// <summary>
    /// Base gizmos class for all AdvancedTurrets related gizmos.
    /// </summary>
    public abstract class BaseGizmos
    {
        [Tooltip("Enables or disables this gizmo from rendering.")]
        public bool Enabled = true;

        [Tooltip("The primary color used to render this gizmo.")]
        public Color MainColor = AdvancedColors.TransparentWhite;
    }
}