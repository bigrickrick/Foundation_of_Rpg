using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : Pathnode

{
    // Represent all the faces of a block althought it being a gameobject will probably be changed
    [SerializeField] private GameObject upFace;
    [SerializeField] private GameObject downFace;
    [SerializeField] private GameObject leftFace;
    [SerializeField] private GameObject rightFace;
    [SerializeField] private GameObject frontFace;
    [SerializeField] private GameObject backFace;

    public bool AffectedByGravity;
    // fence and other block that you can't normally jump on
    public bool CanBeJumpedOn;

    // how long it would take for something to destroy it
    public int BlockHealth;

    public void OnDestroy()
    {
        // drop block pickup item
    }

    public GameObject GetObjectOnBlock()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.51f, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            return hit.collider.gameObject;
        }

        return null;
    }

    

}
