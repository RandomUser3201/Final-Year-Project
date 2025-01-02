using UnityEngine;

public class Node
{
    // [Node Properties]
    public bool walkable;
    public Vector3 worldPosition;
    public Node parent;
    public bool isPathNode;

    // [Grid Position]
    public int gridX;
    public int gridY;

    // [Pathfinding Costs]
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    
    // Node initialization
    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;

        // Indicates whether this node is part of the final path
        this.isPathNode = false;
    }
}
