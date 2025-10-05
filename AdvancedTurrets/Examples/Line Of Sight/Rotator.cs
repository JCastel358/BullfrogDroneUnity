using AdvancedTurrets.Utilities;
using UnityEngine;

namespace AdvancedTurrets.Examples.LineOfSight
{
    [AddComponentMenu("Hidden/")]
    public class Rotator : MonoBehaviour
    {
        public Vector3 DegreesSec;

        void Update()
        {
            transform.Rotate(DegreesSec * AdvancedTime.SmartDeltaTime);
        }
    }
}