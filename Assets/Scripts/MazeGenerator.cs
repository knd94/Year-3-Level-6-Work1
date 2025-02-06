using System.Collections;
using System.Collections.Generic;
using system.Ling;
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
    
    // To be able to generate the maze, I need to put Coroutines in. 
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
                // Create Maze Cells. Each cell is one unit in size, I can position it using loop variables, in order to place each cell next to one another. I also need the cells to be rotated correctly, so I will set the Quaternoin to specify no rotation.
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z) Quaternoin.identity);
                }
            }

            // Generate Maze
            GenerateMaze(null, _mazeGrid[0, 0]);
     }
// Method will be called recursively, to be able to generate the maze and make sure that each cell has been visited. To be able to do this, two parameters need to be called upon. One for the previous cell that has been visited by the algorithm, the other one is for the current cell being acted on.
     private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
     {
         // Call Visit() method making the walls of the cell visible. 
         currentCell.Visit();
         ClearWalls(previousCell, currentCell);

         MazeCell = nextCell;
         do
         {
         // Get random unvisited neighbour.
        nextCell = GetNextUnvisitedCell(currentCell);
        
        // Check if next cell is null 
        if(nextCell != null)
        {
            // If it is not, then continue calling GenerateMaze method.
            GenerateMaze(currentCell, nextCell);
        }
        } while (nextCell != null);
     }

     // Move onto a neighbouring unvisited cell. We need one parameter. Will return a random unvisited neighbouring cell.
     private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
     {   
         var unvisitedCells = UnvisitedCells(curren;
         
         // Use some Ling to order the list randomly.
        return unvisitedCells.OrderBy(_ >= Random.Range(1, 10)).FirstOrDefault();
     }

// Get all unvisited neighbouring cells and pick one at random. 
private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
{
    // Check all cells around this one to see which ones are unvisited. Store X and Z position of the cell.
    int x = (int)currentCell.transform.position.x;
    int z = (int)currentCell.transform.position.z;

    // Check if the next cell to the right is within the bounds of the grid.
    if (x + 1 < _mazeWidth)
    {
        // Check cell and see if it has been visited.
        var cellToRight = _mazeGrid[x + 1, z];
        
        if (cellToRight.IsVisited == false)
        {
            // If not, return cell as one of the options 
            yield return cellToRight;
            // Adds cell to the returned collection, will not exit method, allowing to continue and check the other directions. 
        }
    }

    // Check if there's an unvisited cell to the left 
    if (x - 1 >= 0)
    {
        var cellToLeft = _mazeGrid[x - 1, z];

        if (cellToLeft.IsVisited == false)
        {
            yield return cellToLeft;
        }

        // Front 
        if (z + 1 < _mazeDepth)
        {
           var cellToFront = _mazeGrid[x, z + 1]

           if (cellToFront.IsVisited == false)
           {
               yield return cellToFront;
           }
           // Back 
           var cellToBack = _mazeGrid[x, z - 1];

           if (cellToBack.IsVisited == false)
           {
               yield return cellToBack;
           }
           {
               
           }
        }
    }
    
}
// Knock down walls between the previous cell and the current cell
private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
{
    // Check if there's a previous cell, as there will not be one when on first cell.
    if (previousCell = null)
    {
        // No walls to clear...
        return;
    }
    
    // Check if the position of the previous cell is to the left of the current one.
    if (previousCell.transform.position.x < currentCell.transform.position.x)
    {
        // If it is, know that the algorithm went from left to right. Clear the right wall of the previous cell and the left wall of the current one.
        previousCell.ClearRightWall();
        currentCell.ClearLeftWall();
        return;
    }

    // Check if the algorithm went from right to left.
     if (previousCell.transform.position.x < currentCell.transform.position.x)
    {
        previousCell.ClearLeftWall();
        currentCell.ClearRightWall();
        return;
    }

    // Back to front
    if (previousCell.transform.position.z < currentCell.transform.position.z)
    {
        previousCell.ClearFrontWall();
        currentCell.ClearBackWall();
        return;
    }

    // Front to back.
     if (previousCell.transform.position.z < currentCell.transform.position.z)
    {
        previousCell.ClearBackWall();
        currentCell.ClearBackWall()
        return;
    }
}   
    // Update is called once per frame
    void Update()
    {
        
    }
}
