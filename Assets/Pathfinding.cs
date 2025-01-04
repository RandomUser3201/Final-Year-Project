using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    // [References]
    public Transform seeker;
    public Transform target;
    private NavMeshAgent agent;
    private EnemyAI enemyAI;

    // [Pathfinding Setup]
    private Grid grid;
    public List<Node> path;

    // [Pathing and Movement]
    public int currentNodeIndex = 0;
    private Vector3 lastTargetPosition;
    private float pathThreshold = 1f;
    private float pathRecalcCooldown = 0.2f;
    private float nextPathRecalcTime;
    public float speed = 2f;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        agent = seeker.GetComponent<NavMeshAgent>();

        if (grid == null)
        {
            Debug.LogError("Grid component is missing!");
        }
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }

        lastTargetPosition = target.position;
    }

    void Update()
    {
        // Check if it's time to recalculate the path based on the target's movement.
        if (Time.time >= nextPathRecalcTime && Vector3.Distance(lastTargetPosition, target.position) > pathThreshold)
        {
            FindPath(seeker.position, target.position);
            lastTargetPosition = target.position;
            nextPathRecalcTime = Time.time + pathRecalcCooldown;
        }

        // Continuously follow the path if it exists.
        FollowPath();
    }

    // Finds a path from the seeker to the target using A* algorithm.
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Debug.Log($"Finding path from {seeker.position} to {target.position}");
        Debug.Log($"Enemy {gameObject.name} recalculating path.");

        // Convert the start and target position to a grid node.
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // If either the start or target node is not walkable, we can't calculate the path.
        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.LogWarning("Start or target node is not walkable");
            return;
        }

        // [A* Pathfinding setup]
        // List of nodes that are to be evaluated
        List<Node> openSet = new List<Node>
        {
            startNode
        };

        // Set of nodes that have already been evaluated
        HashSet<Node> closedSet = new HashSet<Node>();

        // List to store the final path
        path = new List<Node>();

        // [A* Algorithm loop]
        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // Find the node with the lowest fCost (f = g + h, where g is the cost from the start node and h is the heuristic)
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost)
                {
                    node = openSet[i];
                }
            }

            // Remove the node from the open set and ad the node to the closed set as it's evaluated
            openSet.Remove(node);
            closedSet.Add(node);

            //  If the target node is found, retrace the path
            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                Debug.Log("Path found with " + path.Count + " nodes.");
                return;
            }

            // Evaluate neighboring nodes
            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                // Skip non-walkable nodes or already evaluated nodes
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Calculate the cost to move to the neighbor
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // Update the gCost and hCost
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    // Set the current node as the parent of the neighbor
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        Debug.LogWarning("Enemy to player pathfinding failed");
    }

    // Retrace the path from end node to start node
    void RetracePath(Node startNode, Node endNode)
    {
        // Clear the existing path.
        path = new List<Node>();
        Node currentNode = endNode;

        // Retrace the path by following the parent nodes.
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Reverse the path to go from start to end and visualize the path
        path.Reverse();
        grid.HighlightPath(path, Color.black);

        // Convert grid path to NavMesh path - set for agent
        if (path.Count > 0)
        {
            List<Vector3> navPath = new List<Vector3>();

            // Check if the position of the node is on the NavMesh
            foreach (var node in path)
            {
                if (NavMesh.SamplePosition(node.worldPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    // Add the position to the NavMesh path
                    navPath.Add(hit.position);
                }
                else
                {
                    // If not on the NavMesh, use the original position
                    navPath.Add(node.worldPosition);
                }
            }

            // Set the agent path to follow the NavMesh path.
            agent.SetPath(CreateNavMeshPath(navPath));
        }
    }

    // Convert list of positions into a NavMeshPath for the agent
    NavMeshPath CreateNavMeshPath(List<Vector3> positions)
    {
        // Calculate the path to the last position
        NavMeshPath navPath = new NavMeshPath();
        agent.CalculatePath(positions[positions.Count - 1], navPath);
        return navPath;
    }

    // Calculate distance between two nodes - Manhattan distance formula
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    public void FollowPath()
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("No path to follow, exiting FollowPath()");
            return;
        }

        // Reference to Enemy AI
        EnemyAI enemyAI = seeker.GetComponent<EnemyAI>();

        // Ensure this is only called in Chase state
        if (enemyAI.GetCurrentState() != EnemyAI.EnemyState.Chase)
        {
            Debug.LogWarning("Not in chase state, stopping FollowPath()");
            return;
        }

        // Move through the path
        if (currentNodeIndex < path.Count)
        {
            Node targetNode = path[currentNodeIndex];
            Vector3 targetPosition = targetNode.worldPosition;

            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(seeker.position, targetPosition) < 1f)
            {
                currentNodeIndex++;
                path.RemoveAt(0);
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