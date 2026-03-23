using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellPlacer : MonoBehaviour
{
    [SerializeField] private GridGizmos gridGizmos;

    private Grid _grid;
    private Grid Grid => gridGizmos.Grid;

    public GridGizmos GridGizmos => gridGizmos;


    // Хранилище объектов по координатам сетки (локальные координаты)
    private readonly Dictionary<Vector2Int, IGridObject> _cells = new();
    public Dictionary<Vector2Int, IGridObject> Cells => _cells;

    // События
    public event Action<Vector2Int, IGridObject> OnPlaced;
    public event Action<Vector2Int, IGridObject> OnRemoved;
    public event Action<Vector2Int, Vector2Int, IGridObject> OnMoved;

    #region Public API (Grid coords)

    public bool PlaceObject(IGridObject obj, Vector2Int cell)
    {
        if (!CanPlaceObject(obj, cell))
            return false;

        _cells[cell] = obj;
        obj.OnPlaced(this, cell);
        OnPlaced?.Invoke(cell, obj);
        return true;
    }

    public bool CanPlaceObject(IGridObject obj, Vector2Int cell)
    {
        if (!IsCellValid(cell))
            return false;

        if (!CellIsEmpty(cell, obj))
            return false;

        if (obj.MyPlaces != null)
            foreach (var offset in obj.MyPlaces)
            {
                if (!IsCellValid(cell + offset) || !CellIsEmpty(cell + offset, obj))
                    return false;
            }

        return true;
    }

    public bool CellIsEmpty(Vector2Int cell, params IGridObject[] ignore)
    {
        if (_cells.ContainsKey(cell) && (ignore == null || !ignore.Contains(_cells[cell])))
        {
            return false;
        }

        foreach (var gridObject in _cells.Values)
        {
            if (ignore != null && ignore.Contains(gridObject))            
                continue;            

            Vector2Int[] offsets = gridObject.MyPlaces;
            if(offsets == null || offsets.Length == 0) 
                continue;

            foreach (var offset in offsets)
            {
                if (cell == gridObject.CellPosition + offset)
                {
                    //Debug.Log($"cell == gridObject.CellPosition + offset {(gridObject as MonoBehaviour).name} {offset}");
                    return false;
                }
            }
        }

        return true;
    }

    public bool RemoveObject(Vector2Int cell)
    {
        if (!_cells.TryGetValue(cell, out var obj))
            return false;

        _cells.Remove(cell);
        obj.OnRemoved(this, cell);
        OnRemoved?.Invoke(cell, obj);
        return true;
    }

    public bool MoveObject(Vector2Int from, Vector2Int to)
    {
        if (!_cells.TryGetValue(from, out var obj))
            return false;

        if (!CanPlaceObject(obj, to))
        {
            //Debug.Log($"!CanPlaceObject(obj, to)");
            return false;
        }

        _cells.Remove(from);
        _cells[to] = obj;

        obj.OnMoved(this, from, to);
        OnMoved?.Invoke(from, to, obj);
        return true;
    }

    public bool MoveObject(IGridObject obj, Vector2Int to, bool canPlace = true)
    {
        Vector2Int? pos = GetPositionOrNull (obj);

        if(pos != null)
            return MoveObject(pos.Value, to);

        if (canPlace)
            return PlaceObject(obj, to);
        else
            return false;
    }

    private Vector2Int? GetPositionOrNull(IGridObject obj)
    {
        foreach (var cell in _cells.Keys)
        {
            if (_cells[cell] == obj)
                return cell;
        }
        return null;
    }

    public bool MoveOrMixed(Vector2Int cellA, Vector2Int cellB, out List<IGridObject> movedObjects, bool invokeInsideEvents = true)
    {
        movedObjects = new List<IGridObject>();

        // Проверяем наличие объектов
        _cells.TryGetValue(cellA, out var objA);
        _cells.TryGetValue(cellB, out var objB);

        // Добавляем все найденные объекты в лист
        if (objA != null) movedObjects.Add(objA);
        if (objB != null) movedObjects.Add(objB);

        // Меняем объекты местами
        if (objA != null)
            _cells[cellB] = objA;
        else
            _cells.Remove(cellB);

        if (objB != null)
            _cells[cellA] = objB;
        else
            _cells.Remove(cellA);

        // Вызываем события
        if (invokeInsideEvents)
        {
            if (objA != null)
            {
                objA.OnMoved(this, cellA, cellB);
                OnMoved?.Invoke(cellA, cellB, objA);
            }

            if (objB != null)
            {
                objB.OnMoved(this, cellB, cellA);
                OnMoved?.Invoke(cellB, cellA, objB);
            }
        }

        // Если ни один объект не найден — возвращаем false
        return movedObjects.Count > 0;
    }

    public IGridObject GetObject(Vector2Int cell, bool useMyPlaces = false)
    {
        IGridObject gridObject = null;

        if(useMyPlaces)
            gridObject = _cells.FirstOrDefault((gridOb) =>
                {
                    IGridObject value = gridOb.Value;

                    if(value == null) return false;
                    else if(value.CellPosition == cell) return true;

                    foreach (var offset in value.MyPlaces)                    
                        if(value.CellPosition +  offset == cell) return true;                    
                    return false;
                }).Value;
        else
            _cells.TryGetValue(cell, out gridObject);

        return gridObject;
    }

    public void ClearAll()
    {
        foreach (var kvp in _cells)
        {
            kvp.Value.OnRemoved(this, kvp.Key);
            OnRemoved?.Invoke(kvp.Key, kvp.Value);
        }

        _cells.Clear();
    }

    public Dictionary<Vector2Int, IGridObject> GetAllCells()
    {
        return _cells;
    }

    #endregion

    #region World helpers

    /// <summary>
    /// Преобразует world-позицию в координату сетки.
    /// Возвращает null, если позиция вне доски.
    /// </summary>
    public Vector2Int? WorldToCell(Vector3 worldPosition)
    {
        if (Grid == null || gridGizmos == null)
            return null;

        Vector3Int gridCell = Grid.WorldToCell(worldPosition);

        Vector2Int localCell = new(
            gridCell.x - gridGizmos.StartPoint.x,
            gridCell.y - gridGizmos.StartPoint.y
        );

        if (!IsCellValid(localCell))
            return null;

        // Подтверждение через центр клетки
        Vector3 center = gridGizmos.GetCellCenterWorld(localCell.x, localCell.y);
        Vector3 half = Grid.cellSize * 0.5f;

        //if (Mathf.Abs(worldPosition.x - center.x) > half.x ||
        //    Mathf.Abs(worldPosition.y - center.y) > half.y)        
        //    return null;        

        return localCell;
    }

    /// <summary>
    /// Возвращает центр клетки в мире.
    /// </summary>
    public Vector3 CellToWorld(Vector2Int cell)
    {
        return gridGizmos.GetCellCenterWorld(cell.x, cell.y);
    }

    #endregion

    #region Validation

    public bool IsCellValid(Vector2Int cell)
    {
        if (gridGizmos == null)
            return false;

        return cell.x >= 0 &&
               cell.y >= 0 &&
               cell.x < gridGizmos.Cells.x &&
               cell.y < gridGizmos.Cells.y;
    }

    #endregion

    //Поиск пути-----------------------

    /// <summary>
    /// Ищет путь от start до end с учетом блокирующих клеток и веса клеток.
    /// </summary>
    /// <param name="start">Стартовая клетка</param>
    /// <param name="end">Целевая клетка</param>
    /// <param name="ignoreStartCell">Игнорировать стартовую клетку при проверке на блокировку</param>
    /// <param name="isBlocked">Делегат: true, если клетка блокирована</param>
    /// <param name="getCellCost">Делегат: возвращает стоимость прохождения клетки (по умолчанию 1)</param>
    /// <returns>Список клеток пути или null, если пути нет</returns>
    public List<Vector2Int> FindPath(
        Vector2Int start,
        Vector2Int end,
        bool ignoreStartCell = true,
        Func<IGridObject, Vector2Int, bool> isBlocked = null,
        Func<IGridObject, Vector2Int, int> getCellCost = null
    )
    {
        if (!IsCellValid(start) || !IsCellValid(end))
            return null;

        if (start == end)
            return new List<Vector2Int> { start };

        var openSet = new SimplePriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        openSet.Enqueue(start, Heuristic(start, end));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
                return ReconstructPath(cameFrom, current);

            foreach (var next in GetNeighbours(current))
            {
                if (!IsCellValid(next))
                    continue;

                var obj = GetObject(next);

                // проверка блокировки клетки
                if (!(ignoreStartCell && next == start) && isBlocked?.Invoke(obj, next) == true)
                    continue;

                // стоимость прохождения клетки
                int stepCost = getCellCost?.Invoke(obj, next) ?? 1;
                int tentativeG = gScore[current] + stepCost;

                if (!gScore.TryGetValue(next, out int existingG) || tentativeG < existingG)
                {
                    cameFrom[next] = current;
                    gScore[next] = tentativeG;
                    int fScore = tentativeG + Heuristic(next, end);
                    openSet.Enqueue(next, fScore);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Стандартный поиск пути: блокируют только занятые клетки.
    /// </summary>
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool ignoreStartCell = true)
    {
        return FindPath(
            start,
            end,
            ignoreStartCell,
            isBlocked: (obj, pos) => obj != null, // клетка заблокирована, если есть объект
            getCellCost: null                     // стандартный вес = 1
        );
    }


    private static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static readonly Vector2Int[] Directions =
    {
    Vector2Int.up,
    Vector2Int.down,
    Vector2Int.left,
    Vector2Int.right
};

    private IEnumerable<Vector2Int> GetNeighbours(Vector2Int cell)
    {
        foreach (var dir in Directions)
            yield return cell + dir;
    }

    private List<Vector2Int> ReconstructPath(
        Dictionary<Vector2Int, Vector2Int> cameFrom,
        Vector2Int current
    )
    {
        var path = new List<Vector2Int> { current };

        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    public List<Vector2Int> GetRandomEmptyCells(int count, System.Random rng = null)
    {
        return GetRandomCells(count, 
            (pos) => (_cells.ContainsKey(pos) == false || _cells[pos] == null), 
            rng);
    }

    public List<Vector2Int> GetRandomEmptyANDCells(int count, Func<Vector2Int, bool> testFunc, System.Random rng = null)
    {
        return GetRandomCells(count,
            (pos) => (_cells.ContainsKey(pos) == false || _cells[pos] == null || testFunc.Invoke(pos)),
            rng);
    }

    /// <summary>
    /// Возвращает список случайных пустых клеток
    /// </summary>
    /// <param name="count">Количество клеток, которое нужно выбрать</param>
    /// <param name="testFunc">Какие клетки должны попасть в список</param>
    /// <param name="rng">Опциональный генератор случайных чисел</param>
    public List<Vector2Int> GetRandomCells(int count, Func<Vector2Int, bool> testFunc, System.Random rng = null)
    {
        if (rng == null) rng = new System.Random();

        List<Vector2Int> emptyCells = new List<Vector2Int>();

        // Перебираем все клетки по сетке
        for (int x = 0; x < gridGizmos.Cells.x; x++)
        {
            for (int y = 0; y < gridGizmos.Cells.y; y++)
            {
                Vector2Int pos = new Vector2Int(x + gridGizmos.StartPoint.x, y + gridGizmos.StartPoint.y);
                if (testFunc.Invoke(pos))
                    emptyCells.Add(pos);                
            }
        }

        // Если запрошено больше пустых клеток, чем есть, возвращаем все
        if (count >= emptyCells.Count)
            return new List<Vector2Int>(emptyCells);

        // Перемешиваем список методом Fisher-Yates
        for (int i = emptyCells.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            var tmp = emptyCells[i];
            emptyCells[i] = emptyCells[j];
            emptyCells[j] = tmp;
        }

        // Берем только нужное количество
        return emptyCells.GetRange(0, count);
    }
}