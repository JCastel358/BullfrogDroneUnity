using System;
using AdvancedTurrets.Behaviours.Actuators;
using UnityEngine;

namespace AdvancedTurrets.Visualization
{
    [Serializable]
    public class EndEffectorGizmos : BaseGizmos
    {
        public float Length = 1f;

        public void DrawGizmos(EndEffector endEffector)
        {
            if (!Enabled || endEffector == default)
            {
                return;
            }

            var fromPositon = endEffector.transform.position;
            var toPosition = endEffector.transform.position + endEffector.transform.forward * Length;

            Gizmos.color = MainColor;
            Gizmos.DrawLine(fromPositon, toPosition);
        }
    }
}