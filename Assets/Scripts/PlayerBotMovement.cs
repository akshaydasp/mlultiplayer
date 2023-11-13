using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBotMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform mainBase;
    private int randomIndex;
    public string enemyTag = "Enemy";  // Tag of enemy/player tagged objects

    private NavMeshAgent agent;
   

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("run", true);
        GoToRandomWaypoint();
    }

    void GoToRandomWaypoint()
    {
        if (waypoints.Length == 0) return;
        randomIndex = Random.Range(0, waypoints.Length - 1);
       
        agent.destination = waypoints[randomIndex].position;
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            GoToRandomWaypoint();
        }

    }

}
