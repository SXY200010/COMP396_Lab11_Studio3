using UnityEngine;
using System.Collections.Generic;

public class AStar
{
    private float HeuristicEstimateCost(Node curNode, Node goalNode)
    {
        return (curNode.position - goalNode.position).magnitude;
    }

    public List<Node> FindPath(Node start, Node goal)
    {
        // Open list (priority queue)
        NodePriorityQueue openList = new NodePriorityQueue();
        openList.Enqueue(start);

        // Setup start node
        start.costSoFar = 0f;
        start.fScore = HeuristicEstimateCost(start, goal);

        // Closed list
        HashSet<Node> closedList = new HashSet<Node>();

        Node node = null;

        while (openList.Length != 0)
        {
            // Get lowest fScore node
            node = openList.Dequeue();

            // Goal reached?
            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }

            // Check neighbours
            var neighbours = GridManager.instance.GetNeighbours(node);

            foreach (Node neighbourNode in neighbours)
            {
                if (!closedList.Contains(neighbourNode))
                {
                    float totalCost = node.costSoFar + GridManager.instance.StepCost;
                    float heuristicValue = HeuristicEstimateCost(neighbourNode, goal);

                    // Update node data
                    neighbourNode.costSoFar = totalCost;
                    neighbourNode.fScore = totalCost + heuristicValue;
                    neighbourNode.parent = node;

                    // Add to open list
                    openList.Enqueue(neighbourNode);
                }
            }

            // Finished with this node
            closedList.Add(node);
        }

        if (node == null || node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        // Build final path
        return CalculatePath(node);
    }

    private List<Node> CalculatePath(Node node)
    {
        List<Node> list = new List<Node>();

        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }

        list.Reverse();
        return list;
    }
}
