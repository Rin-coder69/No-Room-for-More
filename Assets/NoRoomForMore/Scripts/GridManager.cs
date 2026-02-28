using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 100;  // 10 meters = 100 tiles
    [SerializeField] private int gridHeight = 100;
    [SerializeField] private float tileSize = 0.1f; // 10 tiles per meter

    [Header("Debug")]
    [SerializeField] private bool showDebugGrid = true;
    [SerializeField] private Color gridColor = Color.green;

    private bool[,] occupiedTiles;

    void Awake()
    {
        occupiedTiles = new bool[gridWidth, gridHeight];
    }

    // Convert world position to grid coordinates
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        int x = Mathf.RoundToInt(localPos.x / tileSize);
        int z = Mathf.RoundToInt(localPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    // Convert grid coordinates to world position
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, 0, gridPos.y * tileSize);
    }

    // Snap world position to grid
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGrid(worldPos);
        return GridToWorld(gridPos);
    }

    // Check if a tile is occupied
    public bool IsTileOccupied(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= gridWidth || gridPos.y < 0 || gridPos.y >= gridHeight)
            return true; // Out of bounds = occupied

        return occupiedTiles[gridPos.x, gridPos.y];
    }

    // Check if furniture can be placed (checks all tiles it would occupy)
    public bool CanPlaceFurniture(Vector2Int gridPos, Vector2Int furnitureSize)
    {
        for (int x = 0; x < furnitureSize.x; x++)
        {
            for (int y = 0; y < furnitureSize.y; y++)
            {
                Vector2Int checkPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (IsTileOccupied(checkPos))
                    return false;
            }
        }
        return true;
    }

    // Occupy tiles (when placing furniture)
    public void OccupyTiles(Vector2Int gridPos, Vector2Int furnitureSize, bool occupy = true)
    {
        for (int x = 0; x < furnitureSize.x; x++)
        {
            for (int y = 0; y < furnitureSize.y; y++)
            {
                Vector2Int tile = new Vector2Int(gridPos.x + x, gridPos.y + y);
                if (tile.x >= 0 && tile.x < gridWidth && tile.y >= 0 && tile.y < gridHeight)
                {
                    occupiedTiles[tile.x, tile.y] = occupy;
                }
            }
        }
    }

    // Debug visualization
    void OnDrawGizmos()
    {
        if (!showDebugGrid) return;

        Gizmos.color = gridColor;

        // Draw grid lines
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * tileSize, 0, 0) + transform.position;
            Vector3 end = new Vector3(x * tileSize, 0, gridHeight * tileSize) + transform.position;
            Gizmos.DrawLine(start, end);
        }

        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = new Vector3(0, 0, z * tileSize) + transform.position;
            Vector3 end = new Vector3(gridWidth * tileSize, 0, z * tileSize) + transform.position;
            Gizmos.DrawLine(start, end);
        }
    }
}
