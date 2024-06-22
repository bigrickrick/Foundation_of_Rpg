using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Node : MonoBehaviour
{

    public bool Taken;
    private void Start()
    {
        if (!OnNavMesh())
        {
            MoveToClosestNavmesh();
        }
    }
    private void Update()
    {
        if (!OnNavMesh())
        {
            MoveToClosestNavmesh();
        }
    }
    private bool OnNavMesh()
    {
        NavMeshHit hit;
        bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 0f, NavMesh.AllAreas);
        return isOnNavMesh;
    }
    private void MoveToClosestNavmesh()
    {
        NavMeshHit hit;
        bool isOnNavMesh = NavMesh.SamplePosition(transform.position, out hit, 1000f, NavMesh.AllAreas);

        if (isOnNavMesh)
        {
            transform.position = hit.position;
            //Debug.Log("Moved node to closest NavMesh position: " + hit.position);
        }
        else
        {
            //Debug.LogWarning("No valid NavMesh position found within the given distance.");
        }
    }


   

}
