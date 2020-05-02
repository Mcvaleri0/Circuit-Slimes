using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordSelector : MonoBehaviour, ICoordSelector
{
    private SelectionManager Manager;

    private Vector2Int BoardCoords; 
    private bool BoardHover = false;

    // Start is called before the first frame update
    void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();
    }

    private bool IgnoreSelection(Ray ray)
    {
        //ignore selecion when doing a gesture 
        if (Lean.Touch.LeanTouch.Fingers.Count >= 2)
        {
            return true;
        }
        //or clicking on ui
        if (Lean.Touch.LeanTouch.Fingers.Count == 1 && Lean.Touch.LeanTouch.Fingers[0].StartedOverGui)
        {
            return true;
        }
        //or just hovering ui
        var screenpos = Camera.main.WorldToScreenPoint(ray.origin);
        if (Lean.Touch.LeanTouch.PointOverGui(screenpos))
        {
            return true;
        }
        return false;
    }

    public Vector2Int GetCoords(Ray ray)
    {

        var board = this.Manager.BoardTransform.GetComponent<BoxCollider>();
        var puzzle = this.Manager.PuzzleController;

        this.BoardHover = false;

        if (board.Raycast(ray, out var hit, 1000) && !this.IgnoreSelection(ray)) {

            this.BoardHover = true;
            this.BoardCoords = Puzzle.Board.LevelBoard.Discretize(hit.point);
            this.BoardCoords = puzzle.Puzzle.Clamp(this.BoardCoords);
        }
        return this.BoardCoords;
    }

    public bool GetHover()
    {
        return this.BoardHover;
    }
}
