using System;
using System.Collections.Generic;
using AdvancedTurrets.Behaviours.Ammunitions;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AdvancedTurrets.Examples.Framerate
{
    [AddComponentMenu("Hidden/")]
    public class Framerate : MonoBehaviour
    {
        [Min(1)]
        public int FrameRate = 5;

        private Color _frameColor;

        void Update()
        {
            Application.targetFrameRate = FrameRate;
            _frameColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        }

        private int _index = -1;
        readonly Dictionary<BaseAmmunition, Tuple<int, Color>> _indices = new Dictionary<BaseAmmunition, Tuple<int, Color>>();
        public void OnAmmunitionFired(BaseAmmunition ammunition)
        {
            _indices[ammunition] = new(_index++, _frameColor);
        }

        void OnDrawGizmos()
        {
            Handles.Label(transform.position, $"{nameof(FrameRate)}={FrameRate}", new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal = new()
                {
                    textColor = Color.white
                },
            });

            foreach (var (ammunition, (index, color)) in _indices)
            {
                if (!ammunition)
                    continue;

                Handles.Label(ammunition.Position, $"{index}", new GUIStyle()
                {
                    alignment = TextAnchor.MiddleLeft,
                    normal = new()
                    {
                        textColor = color
                    },
                });
            }
        }
    }
}