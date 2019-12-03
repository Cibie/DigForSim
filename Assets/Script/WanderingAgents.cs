using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderingAgents : MonoBehaviour
{
    [HideInInspector]
    public float wanderRadius = 10;
    [HideInInspector]
    public float wanderTimer = 1;
    private Transform target;
    [HideInInspector]
    public NavMeshAgent agent;
    private float timer;
    

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Start()
    {
        agent.Warp(gameObject.transform.position);// Put the agent iun the selected position of the NavMesh
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit; 
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask); // It chose the nearest point to the chosen one on the sphere on the NavMesh
        return navHit.position;
    }
}
