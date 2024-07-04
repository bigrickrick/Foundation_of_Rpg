using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractiveTerrain
{
    public Node ladder_end_point;
    public override void interact(Entity entity)
    {
        Debug.Log(entity+" Has reach ladder");

    }
}
