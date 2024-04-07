using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.SteeringBehaviours
{
    public class SteeringAgent: MonoBehaviour
    {
        [Serializable]
        public enum SummingStrategy
        {
            WeightedAverage,
            Prioritized
        }

        public SummingStrategy summingStrategy = SummingStrategy.WeightedAverage;

        [SerializeField] private float mass = 1f;
        public float Mass
        {
            get => mass;
            set
            {
                mass = value;
                InverseMass = 1f / value;
            }
        }

        public float InverseMass { get; private set; }
        public float maxSpeed = 1f;
        public float maxForce = 10f;
        public bool reachedGoal;

        public Vector3 Velocity { get; private set; } = Vector3.zero;
        
        private readonly List<SteeringBehaviour> _steeringBehaviours = new();

        public float angularDampeningTime = 5f;
        public float deadZone = 10f;

        private Vector3 CalculateSteeringForce()
        {
            var totalForce = Vector3.zero;

            foreach (var behaviour in _steeringBehaviours)
            {
                if (!behaviour.enabled) continue;

                var behaviourForce = behaviour.CalculateForce() * behaviour.Weight;

                switch (summingStrategy)
                {
                    case SummingStrategy.WeightedAverage:
                        totalForce += behaviourForce;
                        break;
                    case SummingStrategy.Prioritized:
                        // TODO
                        Debug.LogWarning("Not Implemented");
                        break;
                }
            }

            // Clamp Total Force
            totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
            
            return totalForce;
        }

        private void Start()
        {
            // Force calculation of inverse mass
            Mass = mass;
            
            _steeringBehaviours.AddRange(GetComponentsInChildren<SteeringBehaviour>());
            foreach (var behaviour in _steeringBehaviours)
            {
                behaviour.steeringAgent = this;
            }
        }

        private void Update()
        {
            var steeringForce = CalculateSteeringForce();

            if (reachedGoal)
            {
                Velocity = Vector3.zero;
                return;
            }
            
            var acceleration = steeringForce * InverseMass;

            Velocity += acceleration * Time.deltaTime;

            if (Velocity.sqrMagnitude == 0) return;
            
            // Slow down, Speedy Gonzales
            Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            // Move Agent
            transform.position += Velocity * Time.deltaTime;
            
            // Rotate Agent
            var angle = Vector3.Angle(transform.forward, Velocity);
            if (Mathf.Abs(angle) < deadZone)
            {
                transform.LookAt(transform.position + Velocity);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(Velocity), 
                    Time.deltaTime * angularDampeningTime);
            }
        }
    }
}