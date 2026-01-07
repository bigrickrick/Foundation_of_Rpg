using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    public int gCost;
    public int hCost;
    public int fcost;

    public Pathnode cameFromNode;

    private void Start()
    {
        if (MapLegacy.instance == null)
        {
            Debug.LogError("Map.instance not found! Pathnode cannot initialize.");
            return;
        }

        Vector3Int cell = MapLegacy.instance.WorldToCell(transform.position);
        x = cell.x;
        y = cell.y;
        z = cell.z;
    }


    public Vector3Int GetGridPosition()
    {
        return new Vector3Int(x, y, z);
    }
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public override string ToString()
    {
        return x+ ","+y+"," + z;
    }

    public void CalculateFcost()
    {
        fcost = gCost + hCost;
    }
}
