using System;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridGizmos : MonoBehaviour
{
    [SerializeField] private Vector2Int startPoint;
    [SerializeField] private Vector2Int cells = new(5, 5);
    [SerializeField] private bool isAlwaysDraw = true;

    private Grid _grid;
    public Grid Grid
    {
        get
        {
            if (_grid == null)
                _grid = GetComponent<Grid>();
            return _grid;
        }
    }

    public Vector2Int StartPoint => startPoint;
    public Vector2Int Cells => cells;

    private void OnDrawGizmosSelected()
    {
        if (!isAlwaysDraw)
            return;

        DrawGrid();
    }

    private void OnDrawGizmos()
    {
        if (isAlwaysDraw)
            DrawGrid();
    }

    private void DrawGrid()
    {
        if (Grid == null) return;

        Gizmos.color = Color.cyan;

        for (int x = 0; x < cells.x; x++)
        {
            for (int y = 0; y < cells.y; y++)
            {
                Vector3 center = GetCellCenterWorld(x, y);
                Gizmos.DrawWireCube(center, Grid.cellSize);
            }
        }
    }

    public Vector3 GetCellCenterWorld(int x, int y)
    {
        Vector3Int cell = new(
            startPoint.x + x,
            startPoint.y + y,
            0
        );
        return Grid.GetCellCenterWorld(cell);
    }
}