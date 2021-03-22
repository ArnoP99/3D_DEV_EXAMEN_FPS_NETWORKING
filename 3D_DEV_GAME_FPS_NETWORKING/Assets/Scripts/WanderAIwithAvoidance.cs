using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderAIwithAvoidance : MonoBehaviour
{
    public class WanderSpot
    {
        public GameObject Center;
        public float WanderRadius;
    }

    public class WanderScript
    {
        public List<WanderSpot> Spots;

        private NavMeshAgent agent;

        private void Update()
        {
            if (!hasReachedDestination()) return;
            var newSpot = Spots[Random.Range(0, Spots.Count)];
            agent.SetDestination(RandomPosition(newSpot));
        }

        private bool hasReachedDestination()
        {
            return agent.remainingDistance <= agent.stoppingDistance;
        }

        private Vector3 RandomPosition(WanderSpot spot)
        {
            var randDirection = Random.insideUnitSphere * spot.WanderRadius;

            randDirection += spot.Center.transform.position;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, spot.WanderRadius, -1);

            return navHit.position;
        }
    }
}

