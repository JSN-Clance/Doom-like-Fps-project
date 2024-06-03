using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Reference to the NavMeshAgent component
    private NavMeshAgent agent;

    // Layer mask to specify what is considered ground
    public LayerMask whatIsGround;

    // Player reference
    public Transform player;

    // Patrol destination point and control variables
    public Vector3 destinationPoint;
    private bool destinationSet;
    public float destinationRange;

    // Chasing player variables
    public float chaseRange = 10f;
    private bool isChasing;

    // Minimum distance to consider destination reached
    public float destinationReachedThreshold = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        searchForDestination();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Guarding();
        }
    }

    private void Guarding()
    {
        if (destinationSet)
        {
            agent.SetDestination(destinationPoint);

            // Check if the enemy has reached the destination
            if (!agent.pathPending && agent.remainingDistance <= destinationReachedThreshold)
            {
                destinationSet = false;
            }
        }
        else
        {
            searchForDestination();
        }
    }

    private void searchForDestination()
    {
        float randPositionZ = Random.Range(-destinationRange, destinationRange);
        float randPositionX = Random.Range(-destinationRange, destinationRange);

        destinationPoint = new Vector3(
            transform.position.x + randPositionX,
            transform.position.y,
            transform.position.z + randPositionZ);

        Debug.Log("Searching for destination: " + destinationPoint);

        if (Physics.Raycast(destinationPoint, -transform.up, 2f, whatIsGround))
        {
            Debug.Log("Destination set to: " + destinationPoint);
            destinationSet = true;
        }
        else
        {
            Debug.Log("Invalid destination point: " + destinationPoint);
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
