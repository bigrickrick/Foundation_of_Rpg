using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractiveTerrain
{
    
    public override void interact(Entity entity)
    {
        Debug.Log(entity+" Has reach ladder");
        Vector3 furthestPoint = CalculateFurthestPoint(entity);

        // Move the entity to the furthest point
        entity.InstantMove(furthestPoint);

    }

    private Vector3 CalculateFurthestPoint(Entity entity)
    {
        Vector3 furthestPoint = interactablepoints[0].transform.position;
        float maxDistance = Vector3.Distance(entity.transform.position, furthestPoint);

        foreach (Node point in interactablepoints)
        {
            float distance = Vector3.Distance(entity.transform.position, point.transform.position);
            if (distance > maxDistance)
            {
                furthestPoint = point.transform.position;
                maxDistance = distance;
            }
        }

        return furthestPoint;
    }
}
