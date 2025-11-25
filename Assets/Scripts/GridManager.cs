using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private static GridManager staticInstance = null;

    public static GridManager instance
    {
        get
        {
            if (staticInstance == null)
            {
                staticInstance = FindFirstObjectByType<GridManager>();

                if (staticInstance == null)
                {
                    Debug.LogError(
                        "Could not locate a GridManager object.\n" +
                        "You must have exactly one GridManager in the scene."
                    );
                }
            }
            return staticInstance;
        }
    }

    private void OnApplicationQuit()
    {
        staticInstance = null;
    }

    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public float obstacleEpsilon = 0.2f;

    public bool showGrid = true;
    public bool showObstacleBlocks = true;

    public Node[,] nodes { get; private set; }

    public Vector3 Origin => transform.position;

    public float StepCost => gridCellSize;

    private void Awake()
    {
        ComputeGrid();
    }

    private void ComputeGrid()
    {
        nodes = new Node[numOfColumns, numOfRows];

        for (int col = 0; col < numOfColumns; col++)
        {
            for (int row = 0; row < numOfRows; row++)
            {
                Vector3 cellPos = GetGridCellCenter(col, row);
                Node node = new Node(cellPos);

                Collider[] collisions = Physics.OverlapSphere(
                    cellPos,
                    gridCellSize / 2f - obstacleEpsilon,
                    1 << LayerMask.NameToLayer("Obstacles")
                );

                if (collisions.Length > 0)
                {
                    node.MarkAsObstacle();
                }

                nodes[col, row] = node;
            }
        }
    }

    public Vector3 GetGridCellCenter(int col, int row)
    {
        Vector3 pos = GetGridCellPosition(col, row);
        pos.x += gridCellSize / 2f;
        pos.z += gridCellSize / 2f;
        return pos;
    }

    public Vector3 GetGridCellPosition(int col, int row)
    {
        float x = col * gridCellSize;
        float z = row * gridCellSize;
        return Origin + new Vector3(x, 0f, z);
    }

    public (int, int) GetGridCoordinates(Vector3 pos)
    {
        if (!IsInBounds(pos))
            return (-1, -1);

        int col = (int)Mathf.Floor((pos.x - Origin.x) / gridCellSize);
        int row = (int)Mathf.Floor((pos.z - Origin.z) / gridCellSize);

        return (col, row);
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;

        return pos.x >= Origin.x &&
               pos.x <= Origin.x + width &&
               pos.z >= Origin.z &&
               pos.z <= Origin.z + height;
    }

    public bool IsTraversable(int col, int row)
    {
        return col >= 0 &&
               row >= 0 &&
               col < numOfColumns &&
               row < numOfRows &&
               !nodes[col, row].isObstacle;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> result = new List<Node>();

        var (col, row) = GetGridCoordinates(node.position);

        if (IsTraversable(col - 1, row)) result.Add(nodes[col - 1, row]);
        if (IsTraversable(col + 1, row)) result.Add(nodes[col + 1, row]);
        if (IsTraversable(col, row - 1)) result.Add(nodes[col, row - 1]);
        if (IsTraversable(col, row + 1)) result.Add(nodes[col, row + 1]);

        return result;
    }

    private void OnDrawGizmos()
    {
        if (showGrid)
        {
            DebugDrawGrid(Color.blue);
        }

        Gizmos.DrawSphere(Origin, 0.25f);

        if (nodes == null)
            return;

        if (showObstacleBlocks)
        {
            Vector3 cellSize = new Vector3(gridCellSize, 1f, gridCellSize);
            Gizmos.color = Color.red;

            for (int i = 0; i < numOfColumns; i++)
            {
                for (int j = 0; j < numOfRows; j++)
                {
                    if (nodes[i, j].isObstacle)
                    {
                        Gizmos.DrawCube(GetGridCellCenter(i, j), cellSize);
                    }
                }
            }
        }
    }

    public void DebugDrawGrid(Color color)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;

        for (int row = 0; row <= numOfRows; row++)
        {
            Vector3 start = Origin + new Vector3(0, 0, row * gridCellSize);
            Vector3 end = start + new Vector3(width, 0, 0);
            Debug.DrawLine(start, end, color);
        }

        for (int col = 0; col <= numOfColumns; col++)
        {
            Vector3 start = Origin + new Vector3(col * gridCellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, height);
            Debug.DrawLine(start, end, color);
        }
    }
}
