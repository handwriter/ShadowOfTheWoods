using UnityEngine;
using ThunderWire.Attributes;

namespace UHFPS.Runtime
{
    [InspectorHeader("AI Waypoint")]
    public class AIWaypoint : MonoBehaviour
    {
        public GameObject ReservedBy;

        private void Start()
        {
            Debug.Log(gameObject.name);
        }
    }
}