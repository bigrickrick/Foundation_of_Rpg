using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveTerrain : Modifiers, Interactable
{
    public float interactionDistance;
    public List<Node> interactablepoints;

    void Start()
    {
        currentType = Modifiers.Type.Interactiveterrain;
    }

    public abstract void interact(EntityLegacy entity);

    public virtual bool IsEntityCloseEnough(Vector3 entityPosition)
    {
        float distance = Vector3.Distance(transform.position, entityPosition);
        return distance <= interactionDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    public virtual Vector3 InteractablePoint(EntityLegacy entity)
    {
        
        Vector3 closestPoint = interactablepoints[0].transform.position;
        float minDistance = Vector3.Distance(entity.transform.position, closestPoint);

        foreach (Node point in interactablepoints)
        {
            float distance = Vector3.Distance(entity.transform.position, point.transform.position);
            if (distance < minDistance)
            {
                closestPoint = point.transform.position;
                minDistance = distance;
            }
        }

        return closestPoint;
    }
}
