using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingLegacy : MonoBehaviour
{
    public static PathfindingLegacy instance;
    private MapLegacy grid;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_THREE_DIAGONAL_COST = 17;
    [SerializeField] private float gridSpacing = 2f;
    private List<Pathnode> openList;
    private List<Pathnode> closeList;

    private void Start()
    {
        instance = this;
        grid = MapLegacy.instance;

    }

    public List<Pathnode> FindPath(Block startBlock, Block endBlock)
    {
        if (startBlock == null || endBlock == null)
        {
            Debug.LogWarning("Start or End Block is null in FindPath().");
            return null;
        }

        Pathnode startNode = startBlock.GetComponent<Pathnode>();
        Pathnode endNode = endBlock.GetComponent<Pathnode>();

        if (startNode == null || endNode == null)
        {
            Debug.LogWarning("Start or End Block does not contain a Pathnode component.");
            return null;
        }

        openList = new List<Pathnode>() { startNode };
        closeList = new List<Pathnode>();

        // Reset all pathnodes
        foreach (Block block in MapLegacy.instance.BlockMapList)
        {
            if (block == null) continue;
            Pathnode pathNode = block.GetComponent<Pathnode>();
            if (pathNode == null) continue;

            pathNode.gCost = int.MaxValue;
            pathNode.CalculateFcost();
            pathNode.cameFromNode = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFcost();

        while (openList.Count > 0)
        {
            Pathnode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }
            
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (Pathnode neighbourNode in GetNeighbourList(currentNode))
            {
                if(neighbourNode != null)
                {
                    if (closeList.Contains(neighbourNode)) continue;

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFcost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
                
            }
        }

        // No path found
        return null;
    }

    private List<Pathnode> GetNeighbourList(Pathnode currentnode)
    {
        List<Pathnode> neighbourlist = new List<Pathnode>();
        Vector3 currentPos = currentnode.transform.position;

        // Real world spacing between cube centers is 1
        float spacing = 2f;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    Vector3 checkPos = currentPos + new Vector3(x, y, z) * spacing;

                    Block neighborBlock = MapLegacy.instance.ReturnCellObject(checkPos);
                    if (neighborBlock != null)
                    {
                        Pathnode neighborNode = neighborBlock.GetComponent<Pathnode>();
                        if (neighborNode != null)
                        {
                            Vector3 abovePos = neighborBlock.transform.position + Vector3.up * spacing;
                            Block blockAbove = MapLegacy.instance.ReturnCellObject(abovePos);

                            
                            neighbourlist.Add(neighborNode);


                        }
                    }

                }
            }
        }

        Debug.Log($"[Neighbours] Found {neighbourlist.Count} neighbors for node at {currentPos}");
        return neighbourlist;
    }





    private Pathnode GetNode(int x, int y, int z)
    {

        Block cellObject = MapLegacy.instance.ReturnCellObject(new Vector3(x, y, z));
        if (cellObject == null)
        {
            //Debug.LogError($"[Pathfinding] cellObject is null at ({x},{y},{z})");
            return null;
        }

        Pathnode node = cellObject.GetComponent<Pathnode>();
        if (node == null)
        {
            //Debug.LogError($"[Pathfinding] No Pathnode component found at ({x},{y},{z})");
            return null;
        }

        return node;
    }

    private List<Pathnode> CalculatePath(Pathnode endnode)
    {
        List<Pathnode> path = new List<Pathnode>();
        Pathnode currentnode = endnode;

        while (currentnode != null)
        {
            path.Add(currentnode);
            currentnode = currentnode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    
    private int CalculateDistanceCost(Pathnode a, Pathnode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int zDistance = Mathf.Abs(a.z - b.z);


        List<int> distances = new List<int> { xDistance, yDistance, zDistance };
        distances.Sort();


        int threeAxisMoves = distances[0];            
        int twoAxisMoves = distances[1] - distances[0];       
        int straightMoves = distances[2] - distances[1];      

        return threeAxisMoves * MOVE_THREE_DIAGONAL_COST
             + twoAxisMoves * MOVE_DIAGONAL_COST
             + straightMoves * MOVE_STRAIGHT_COST;
    }


    private Pathnode GetLowestFCostNode(List<Pathnode> pathnodelist)
    {
        Pathnode lowestFCostNode = pathnodelist[0];

        for (int i = 1; i < pathnodelist.Count; i++)
        {
            if (pathnodelist[i].fcost < lowestFCostNode.fcost)
            {
                lowestFCostNode = pathnodelist[i];
            }
            else if (pathnodelist[i].fcost == lowestFCostNode.fcost)
            {
                if (pathnodelist[i].hCost < lowestFCostNode.hCost)
                {
                    lowestFCostNode = pathnodelist[i];
                }
            }
        }

        return lowestFCostNode;
    }
}
