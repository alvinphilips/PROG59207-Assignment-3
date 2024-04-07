using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class FollowPathSteeringBehaviour: ArriveSteeringBehaviour
    {
        [SerializeField] protected float ehCloseEnoughDistance = 0.5f;
        
        protected readonly Queue<Vector3> QueuedPoints = new();

        public override Vector3 CalculateForce()
        {
            if (!QueuedPoints.TryPeek(out var currentTarget))
            {
                RefreshQueue();
                return Vector3.zero;
            }
            
            if (Vector3.Distance(currentTarget, transform.position) < ehCloseEnoughDistance)
            {
                QueuedPoints.Dequeue();

                if (!QueuedPoints.TryPeek(out currentTarget))
                {
                    RefreshQueue();
                    return Vector3.zero;
                }
            }

            Target = currentTarget;

            return CalculateArriveForce();
        }

        protected virtual void RefreshQueue()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                QueuedPoints.Enqueue(transform.GetChild(i).position);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            var startPoint = transform.position;
            foreach (var point in QueuedPoints)
            {
                Debug.DrawLine(startPoint, point, Color.black);
                startPoint = point;
            }
        }
    }
}