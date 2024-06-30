using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : InteractiveTerrain
{
    public Node ladder_end_point;
    public override void interact()
    {
        Debug.Log(" Has reach ladder");

    }
}
