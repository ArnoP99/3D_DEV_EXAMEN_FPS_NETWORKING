using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    public float FollowDistance;
    public NavMeshAgent patroler;
    private GameObject[] players;
    public Transform[] points;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
                patrol = !follow && points.Length > 0;

                if (follow)
                {
                    agent.stoppingDistance = 2.5f;
                    agent.SetDestination(item.transform.position);
                }

                if (patrol)
                {
                    agent.stoppingDistance = 0f;
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