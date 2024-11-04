using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker; // enemy
    public Transform target; // player
    public float speed = 2f;

    private Grid grid;
    private List<Node> path;
    private int currentNodeIndex = 0;
    Vector3 lastTargetPosition;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("Grid component is missing!");
            Debug.LogError("Grid component is not available.");
        }
        
        lastTargetPosition = target.position;
    }

    void Update()
    {
        if (Vector3.Distance(lastTargetPosition, target.position) > 0.1f)
        {
            FindPath(seeker.position, target.position);
            lastTargetPosition = target.position;
        }
        FollowPath();
    }


    // Finds a path from the seeker to the target
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log($"Finding path from {seeker.position} to {target.position}");

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.LogWarning("Start or target node is not walkable");
            return;
        }

        // open and closed lists for A* search
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();
        path = new List<Node>();

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost)
                    node = openSet[i];
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                Debug.Log("Path found with " + path.Count + " nodes.");
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        Debug.LogWarning("Enemy to player pathfinding failed");
    }

    // Retrace the path from end node to start node
    void RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        currentNodeIndex = 0;

        if (path.Count == 0)
        {
            Debug.LogWarning("Path is empty retracing.");
        }
        else
        {
            Debug.Log($"Path retraced with" +  path.Count + " nodes.");
            foreach (var node in path)
            {
                Debug.Log($"Node in path: {node.worldPosition}");
            }
        }
    }

    // moves the seeker along the path
    void FollowPath()
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path is empty, cannot follow.");
            return;
        }

        if (currentNodeIndex < path.Count)
        {
            Node currentNode = path[currentNodeIndex];
            Vector3 targetPosition = currentNode.worldPosition;

            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(seeker.position, targetPosition) < 0.1f)
            {
                Debug.Log("Reached node " + currentNodeIndex);
                currentNodeIndex++;
            }
        }
        else
        {
            Debug.Log("Reached target!");
        }
    }


    // calculates the distance between two nodes
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }
}
