using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractiveTerrain
{
    public Node ladder_end_point_1;
    public Node ladder_end_point_2;
    public override void interact(Entity entity)
    {
        Debug.Log(entity+" Has reach ladder");
        Node closestNode = CalculateNodeToMoveTo(entity);

        // Move the entity to the closest node
        entity.InstantMove(closestNode.transform.position);

    }

    public Node CalculateNodeToMoveTo(Entity entity)
    {
        
        Vector3 entityPosition = entity.transform.position;
        Vector3 position1 = ladder_end_point_1.transform.position;
        Vector3 position2 = ladder_end_point_2.transform.position;

        
        float distanceToNode1 = Vector3.Distance(entityPosition, position1);
        float distanceToNode2 = Vector3.Distance(entityPosition, position2);

        
        if (distanceToNode1 < distanceToNode2)
        {
            return ladder_end_point_2;
        }
        else
        {
            return ladder_end_point_1;
        }
    }
}
