using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyfollow : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent enemy;
    private GameObject[] players;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            enemy.SetDestination(item.transform.position);
        }
       
    }
}
