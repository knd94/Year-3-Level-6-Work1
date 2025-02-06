using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    // Want the generator to be able to create a grid of maze cells so I have to add the Prefabs
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    // Size of Maze
    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _MazeDepth;

    // To hold the grid of cells, I have to create a 2D array.
    private MazeCell[,] _mazeGrid;
    
    // Start is called before the first frame update
    void Start()
    {
        // Start array using the Maze Width and Depth
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        // Populate array with Maze Cells. To do this, I will create a loop that goes from 0 to the width of the maze.
        for (int x = 0; x < _mazeWidth; x++)
        {
            // Another loop, this time the depth
            for (int z = 0; z < _mazeDepth; z++)
            {
                // Create Maze Cells. Each cell is one unit in size, I can position it using loop variables, in order to place each cell next to one another. We also need the cells to be rotated correctly, so I will set the Quaternoin to specify no rotation.
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z) Quaternoin.identity);
                }
            }
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
