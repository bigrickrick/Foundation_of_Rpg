using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveTerrain : Modifiers, Interactable
{
    
    public float interactionDistance; 

   
    void Start()
    {
        currentType = Modifiers.Type.Interactiveterrain;
    }
    public abstract void interact(Entity entity);
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
}
