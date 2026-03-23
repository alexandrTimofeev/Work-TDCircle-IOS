using System;
using System.Collections.Generic;
using UnityEngine;

public class CellPlacerGODrawer : MonoBehaviour
{
    [SerializeField] private GameObject cellPref;
    private Dictionary<Vector2Int, GameObject> cellsGO = new Dictionary<Vector2Int, GameObject>();
    public IReadOnlyDictionary<Vector2Int, GameObject> CellsGO => cellsGO;
    [SerializeField] private CellPlacer cellPlacerOnStart;

    private void Start()
    {
        if(cellPlacerOnStart != null)
            Init(cellPlacerOnStart);   
    }

    public void Init(CellPlacer cellPlacer)
    {
        Clear();
        Vector2Int sizeGrid = cellPlacer.GridGizmos.Cells;

        for (int x = 0; x < sizeGrid.x; x++)
        {
            for (int y = 0; y < sizeGrid.y; y++)
            {
                GameObject ngo = Instantiate(cellPref, transform);
                Vector2Int pos = new(x, y);
                ngo.transform.position = cellPlacer.CellToWorld(pos);
                cellsGO.Add(pos, ngo);
            }
        }
    }

    private void Clear()
    {
        foreach(var cell in cellsGO.Values)
        {
           Destroy(cell);
        }

        cellsGO.Clear();
    }
}
