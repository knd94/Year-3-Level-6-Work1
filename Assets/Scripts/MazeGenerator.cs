using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton maze generator that builds a procedural maze and provides utility methods
/// for ML Agents (random start/goal positions, maze dimensions).
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance { get; private set; }

    [Header("Maze Cell Prefab")]
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [Header("Maze Dimensions")]
    [SerializeField]
    private int _mazeWidth = 10;
    [SerializeField]
    private int _mazeDepth = 10;

    /// <summary>
    /// Logical grid of cells instantiated in the scene.
    /// </summary>
    private MazeCell[,] _mazeGrid;

    /// <summary>
    /// Exposed for agents to normalize observations.
    /// </summary>
    public Vector2Int MazeSize => new Vector2Int(_mazeWidth, _mazeDepth);

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BuildGrid();
        GenerateMaze(null, _mazeGrid[0, 0]);
    }

    /// <summary>
    /// Instantiates the grid of MazeCell prefabs.
    /// </summary>
    private void BuildGrid()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                Vector3 pos = new Vector3(x + 0.5f, 0f, z + 0.5f);
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, pos, Quaternion.identity, transform);
            }
        }
    }

    /// <summary>
    /// Recursive backtracking maze generation.
    /// </summary>
    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisited = GetUnvisitedNeighbors(currentCell);
        return unvisited.OrderBy(_ => Random.value).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedNeighbors(MazeCell cell)
    {
        int x = Mathf.FloorToInt(cell.transform.position.x);
        int z = Mathf.FloorToInt(cell.transform.position.z);

        // Right
        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited)
            yield return _mazeGrid[x + 1, z];
        // Left
        if (x - 1 >= 0 && !_mazeGrid[x - 1, z].IsVisited)
            yield return _mazeGrid[x - 1, z];
        // Forward
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited)
            yield return _mazeGrid[x, z + 1];
        // Back
        if (z - 1 >= 0 && !_mazeGrid[x, z - 1].IsVisited)
            yield return _mazeGrid[x, z - 1];
    }

    private void ClearWalls(MazeCell prev, MazeCell curr)
    {
        if (prev == null) return;

        Vector3 p = prev.transform.position;
        Vector3 c = curr.transform.position;

        if (p.x < c.x)
        {
            prev.ClearRightWall(); curr.ClearLeftWall();
        }
        else if (p.x > c.x)
        {
            prev.ClearLeftWall(); curr.ClearRightWall();
        }
        else if (p.z < c.z)
        {
            prev.ClearFrontWall(); curr.ClearBackWall();
        }
        else if (p.z > c.z)
        {
            prev.ClearBackWall(); curr.ClearFrontWall();
        }
    }

    /// <summary>
    /// Returns a random position on an empty (visited or unvisited) cell.
    /// </summary>
    public Vector3 GetRandomEmptyCellWorldPos()
    {
        int x = Random.Range(0, _mazeWidth);
        int z = Random.Range(0, _mazeDepth);
        // Center of cell at y=0.5f so agent sits above floor
        return new Vector3(x + 0.5f, 0.5f, z + 0.5f);
    }

    /// <summary>
    /// For now, goal can be any random cell too.
    /// </summary>
    public Vector3 GetRandomGoalCellWorldPos() => GetRandomEmptyCellWorldPos();
}
