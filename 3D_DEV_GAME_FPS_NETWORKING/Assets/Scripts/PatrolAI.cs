using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class PatrolAI : MonoBehaviour
{
    public float FollowDistance;
    public NavMeshAgent patroler;
    private GameObject[] players;
    public Transform[] points;

    private int destPoint = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = true;

        GotoNextPoint();
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (agent.enabled)
            {
                float dist = Vector3.Distance(item.transform.position, this.transform.position);

                bool patrol = false;
                bool follow = (dist < FollowDistance);


                if (follow)
                {
                    agent.SetDestination(item.transform.position);
                }
                patrol = !follow && points.Length > 0;


                if (patrol)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                        GotoNextPoint();
                }
            }
        }
        
    }


    void GotoNextPoint()
    {
        if (points.Length > 0)
        {
            agent.destination = points[Random.Range(0, 43)].position;

        }
    }
}
