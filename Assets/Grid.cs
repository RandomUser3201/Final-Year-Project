using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class Grid : MonoBehaviour
{
    // [Public variables - Grid Management]
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public Vector3 gridOrigin;

    public float nodeRadius;
    public float nodeSize;
    float nodeDiameter;

    // Properties to easily access the grid's world size in X and Y directions
    public float gridWorldWidth => gridSizeX * nodeSize;
    public float gridWorldHeight => gridSizeY * nodeSize;
    public float gridOriginX => gridOrigin.x;
    public float gridOriginZ => gridOrigin.z;

    int gridSizeX, gridSizeY;
    Node[,] grid;

    void Awake()
    {
        // Calculate the diameter based on the radius
        nodeDiameter = nodeRadius * 2;

        // Calculate how many nodes fit in the X and Y direction.
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void Update()
    {
        // If grid is created, draw debug lines for unwalkable nodes
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (!n.walkable)
                {
                    Debug.DrawLine(n.worldPosition, n.worldPosition + Vector3.up * 2, Color.red, 0.1f);
                }
            }
        }
    }

    void CreateGrid()
    {
        // Initializes the grid with the appropriate size
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Iterates through each position in the grid and create nodes
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the world position for each node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeRadius * 2 + nodeRadius) + Vector3.forward * (y * nodeRadius * 2 + nodeRadius);

                // Check if the current position is walkable - sphere cast to detect obstacles
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Function to get the neighbors of a specific node.
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Iterate through each position in the grid and create nodes.
        for (int x = -1; x <= 1; x++)
        {
            // Loop through the neighboring nodes in both X and Y directions)
            for (int y = -1; y <= 1; y++)
            {
                // Skip the current node
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Check if the neighboring node is within the grid bounds
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    // Add the valid neighbor to the list
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Get position of the grid's bottom left corner
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Calculate the position as a percentage of the grid size
        float percentX = (worldPosition.x - worldBottomLeft.x) / gridWorldSize.x;
        float percentY = (worldPosition.z - worldBottomLeft.z) / gridWorldSize.y;

        // Clamp the values - ensure within bounds
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Converts percentages into grid coordinates and returns corresponding node
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public void HighlightPath(List<Node> path, Color uniquecolor)
    {
        foreach (Node n in grid)
        {
            // Reset the path for all nodes
            n.isPathNode = false;
        }

        if (path != null)
        {
            foreach (Node n in path)
            {
                // Mark node as part of the path and draw a line for the path nodes
                n.isPathNode = true;
                Debug.DrawLine(n.worldPosition, n.worldPosition + Vector3.up * 2, uniquecolor, 0.5f);
            }
        }
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // If the grid is created, cubes drawn for each node to visualize the grid structure
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                // Color the node white if it's walkable, or red if it's unwalkable
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                if (n.isPathNode)
                {
                    // Highlight path nodes in green
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}

/*References:

[Pathfinding]
Sebastian League (2014). A* Pathfinding (E03: algorithm implementation). [online] YouTube. Available at: https://www.youtube.com/watch?v=mZfyt03LDH4&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3 [Accessed 3 Jan. 2025].

Aron G (2017). Get Started Tutorial - A* Pathfinding Project (4.1+). [online] YouTube. Available at: https://www.youtube.com/watch?v=5QT5Czfe0YE&list=PLKwzRvX1eGwHXz5D0nZUFx3fjHZBdyA6s [Accessed 3 Jan. 2025].

Aron G (2017). Get Started Tutorial - A* Pathfinding Project (4.0 and earlier). [online] YouTube. Available at: https://www.youtube.com/watch?v=OpgUcYzRpwM&list=PLKwzRvX1eGwHXz5D0nZUFx3fjHZBdyA6s&index=2 [Accessed 3 Jan. 2025].

Code Monkey (2021). How to use Unity NavMesh Pathfinding! (Unity Tutorial). [online] Available at: https://www.youtube.com/watch?v=atCOd4o7tG4 [Accessed 3 Jan 2025].

Sebastian League (2014). A* Pathfinding Tutorial (Unity) - YouTube. [online] Available at: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW. [Accessed 3 Jan 2025].*/