using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void interact(Entity entity);

    bool IsEntityCloseEnough(Vector3 entityPosition);
    
    Vector3 InteractablePoint(Entity entity);
}
