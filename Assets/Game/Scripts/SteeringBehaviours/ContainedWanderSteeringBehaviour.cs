using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class ContainedWanderSteeringBehaviour : WanderSteeringBehaviour
    {
        [SerializeField] private Jail containedArea;

        protected override void Start()
        {
            base.Start();

            // Sometimes our own minds are what imprison us
            if (containedArea == null)
            {
                containedArea = steeringAgent.transform.parent.gameObject.AddComponent<Jail>();
            }

            allRoadsLeadToRome = containedArea.transform;
        }

        public override Vector3 CalculateForce()
        {
            if (containedArea.IsContained(transform.position))
            {
                return base.CalculateForce();
            }
            
            Target = allRoadsLeadToRome.position + UtilsMath.RandomInsideCircle(containedArea.radius);
                
            return CalculateSeekForce();
        }
    }
}
