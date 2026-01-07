using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularWorld : MonoBehaviour
{
    // This class is used to create worlds with your mouse more easily

    public Block block;
    public Grid grid;
    public GridInputTest interact;

    private void Update()
    {
        Vector3 selectedPosition = interact.GetSelectedMapPosition();
        Vector3Int cellPosition = grid.WorldToCell(selectedPosition);

        block.transform.position = grid.GetCellCenterWorld(cellPosition);

        if (Input.GetMouseButtonDown(0))
        {
            
            Instantiate(block, block.transform.position, Quaternion.identity);
            
        }
    }
}
