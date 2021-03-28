using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private NavMeshAgent enemy;
    private GameObject[] players;
    private GameObject closestPlayer;
    private float distance = 1000f;

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (Vector3.Distance(item.transform.position, enemy.transform.position) < distance)
            {
                closestPlayer = item;
            }
        }
        enemy.SetDestination(closestPlayer.transform.position);
        distance = 1000f; // Reset distance for next check
    }
}
