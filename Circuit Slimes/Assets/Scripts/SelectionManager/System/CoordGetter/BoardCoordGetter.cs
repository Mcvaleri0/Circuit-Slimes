using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordGetter : MonoBehaviour
{
    private SelectionManager Manager;

    private Vector2Int BoardCoords; 
    private bool BoardHover = false;

    // Start is called before the first frame update
    void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();
    }

    public Vector2Int GetCoords(Ray ray)
    {

        var board = this.Manager.BoardTransform.GetComponent<BoxCollider>();

        this.BoardHover = false;

        if (board.Raycast(ray, out var hit, 1000)) {

            this.BoardHover = true;
            this.BoardCoords = Puzzle.Board.LevelBoard.Discretize(hit.point);
        }
        
        return BoardCoords;
    }

    public bool GetHover()
    {
        return this.BoardHover;
    }
}
