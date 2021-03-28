using UnityEngine;
using UnityEngine.AI;

public class PatrolAI : MonoBehaviour
{
    public float FollowDistance = 7.5f;

    public NavMeshAgent patroler;
    private GameObject[] players;
    private NavMeshAgent agent;

    public Transform[] points;

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

                bool patrol = true;
                bool follow = (dist < FollowDistance);


                if (follow)
                {
                    agent.SetDestination(item.transform.position);
                }
                patrol = !follow && points.Length > 0;
                if (patrol)
                {
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        GotoNextPoint();
                        Debug.Log("test");
                    }
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