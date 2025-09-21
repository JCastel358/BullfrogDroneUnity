using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples.Timeouts
{
    [AddComponentMenu("Hidden/")]
    public class TargetResetter : MonoBehaviour
    {
        CheckTimer _checkTimer = new();

        public float Rate = 2f;
        public GameObject Target;

        void Update()
        {
            if (_checkTimer.HasElapsed())
            {
                Target.SetActive(!Target.activeSelf);
                _checkTimer.Reset(Rate);
            }
        }
    }
}