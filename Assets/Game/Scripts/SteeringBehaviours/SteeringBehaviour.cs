using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public abstract class SteeringBehaviour : MonoBehaviour
    {
        [SerializeField] protected float weight = 1f;
        public float Weight { get => weight; set => weight = value; }
        public Vector3 Target { get; set; } = Vector3.zero;

        public bool useMouseInput = true;

        [HideInInspector] public SteeringAgent steeringAgent;

        protected bool CheckMouseInput()
        {
            if (Input.GetMouseButton(0) && useMouseInput)
            {
                var ray = CameraUtils.Main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100))
                {
                    Target = hit.point;
                    steeringAgent.reachedGoal = false;
                    return true;
                }
            }
            return false;
        }
        public abstract Vector3 CalculateForce();
    }
}
