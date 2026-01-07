using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapLegacy : MonoBehaviour
{
    public Tilemap grid;

    public static MapLegacy instance;
    public List<Block> BlockMapList;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        BlockMapList = new List<Block>();

        foreach (Transform child in grid.transform)
        {
            BlockMapList.Add(child.GetComponentInChildren<Block>());
            
        }
    }
    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        return grid.WorldToCell(worldPos);
    }


    public Block ReturnCellObject(Vector3 worldPos)
    {
        Vector3Int cellPos = grid.WorldToCell(worldPos);
        foreach (Block block in BlockMapList)
        {
            if (block == null) continue;

            Vector3Int blockPos = grid.WorldToCell(block.transform.position);
            if (blockPos == cellPos)
            {
                return block;
            }
        }

        Debug.Log($"[Map] No block found for worldPos {worldPos}, cell {cellPos}");
        return null;
    }


    public Block GetBlockFromClick(RaycastHit hit)
    {
        Block ClickedBlock = hit.collider.GetComponent<Block>();
        return ClickedBlock;
    }
    public Block GetClosetBlockFromEntity(Vector3 entityWorldPos)
    {
        if (BlockMapList == null || BlockMapList.Count == 0)
            return null;

        Block closestBlock = null;
        float closestDistance = Mathf.Infinity;

        foreach (Block block in BlockMapList)
        {
            if (block == null) continue;

            float distance = Vector3.Distance(entityWorldPos, block.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBlock = block;
            }
        }

        return closestBlock;
    }

    public int GetGridWidth()
    {
        return grid.cellBounds.size.x;
    }

    public int GetGridHeight()
    {
        return grid.cellBounds.size.y;
    }

    public int GetGridLength()
    {
        return grid.cellBounds.size.z;
    }

    public void AddBlock(Block block)
    {
        if (block == null)
            return;

        
        block.transform.SetParent(transform);

        
        Vector3 position = block.transform.position;
        block.transform.position = new Vector3(
            Mathf.Round(position.x),
            Mathf.Round(position.y),
            Mathf.Round(position.z)
        );

        
        BlockMapList.Add(block);
    }


}
