using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWalll;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    [SerializeField]
    private GameObject _unvisitedMazeCell;

    // Cheeck to see if Cell has been visited by generation via a boolean.
    public bool IsVisited { get; private set; }

    // Helper Methods
    // To be called whenever a Cell is visited by the algorithm/generator. 
    public void Visit()
    {
        IsVisited = True;
        // Disable the Unvisited Maze Cell, so the walls will become visible. 
        _unvisitedMazeCell.SetActive(false);
    }
}
