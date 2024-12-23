using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker; // enemy
    public Transform target; // player
    public float speed = 2f;

    private Grid grid;
    private List<Node> path;
    private int currentNodeIndex = 0;
    private NavMeshAgent agent;
    private Vector3 lastTargetPosition;
    private float pathThreshold = 1f;

    private bool pathfindingEnabled = false;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pathfindingEnabled = !pathfindingEnabled;
        }

        if (pathfindingEnabled)
        {
            // Recalculate the path if the target has moved significantly
            if (Vector3.Distance(lastTargetPosition, target.position) > pathThreshold)
            {
                FindPath(seeker.position, target.position);
                lastTargetPosition = target.position;
            }
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

        // A* pathfinding
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

        // tested - using this will start make the enemy start from the first node as opposed to the last one, each time the player moves the enemy will go further.
        //currentNodeIndex = Mathf.Min(currentNodeIndex, path.Count - 1);

        grid.HighlightPath(path);


        // pass the path to NavMeshAgent
        if (path.Count > 0)
        {
            List<Vector3> navPath = new List<Vector3>();
            foreach (var node in path)
            {
                navPath.Add(node.worldPosition);
            }

            // follow the calculated path using navmesh
            agent.SetPath(CreateNavMeshPath(navPath));
        }


    }

    // grid path into a NavMeshPath for the agent
    NavMeshPath CreateNavMeshPath(List<Vector3> positions)
    {
        NavMeshPath navPath = new NavMeshPath();
        agent.CalculatePath(positions[positions.Count - 1], navPath);
        return navPath;
    }

    // calculates the distance between two nodes
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    void FollowPath()
    {
        if (path == null || path.Count == 0) return;

        if (currentNodeIndex < path.Count)
        {
            Node targetNode = path[currentNodeIndex];
            Vector3 targetPosition = targetNode.worldPosition;

            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(seeker.position, targetPosition) < 1f)
            {
                currentNodeIndex++;
            }
        }

        else
        {
            seeker.position = Vector3.MoveTowards(seeker.position, target.position, speed * Time.deltaTime);
        }
    }
}
