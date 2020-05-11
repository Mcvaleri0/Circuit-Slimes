using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordSelector : MonoBehaviour, ICoordSelector
{
    private SelectionManager Manager;

    private Vector2Int BoardCoords; 
    private bool BoardHover = false;

    private Transform Selection;

    private Vector2Int BoardCoordsOffset;

    private List<Transform> WhiteList;


    void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();
    }


    //ignore selection thanks to ui or touch gesture
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


    public Transform Check(Ray ray)
    {
        var board = this.Manager.BoardTransform.GetComponent<BoxCollider>();
        var puzzle = this.Manager.PuzzleController.Puzzle;

        this.BoardHover = false;

        this.Selection = null;

        this.BoardCoordsOffset = new Vector2Int(0, 0); 

        if (board.Raycast(ray, out var hit, 1000) && !this.IgnoreSelection(ray)) {

            this.WhiteList = Manager.WhiteList;

            //hover
            this.BoardHover = true;

            //coords
            this.BoardCoords = Puzzle.Board.LevelBoard.Discretize(hit.point);
            this.BoardCoords = puzzle.Clamp(this.BoardCoords);

            //selection
            var piece = puzzle.GetPiece(this.BoardCoords);
            var tile = puzzle.GetTile(this.BoardCoords);

            //get the right transform (piece >> tile)
            if (piece != null)
            {
                var tr = piece.transform;

                //check if filter allows it
                if ( WhiteList == null || (WhiteList.Count != 0 && WhiteList.Contains(tr)))
                {
                    this.Selection = tr;

                    this.BoardCoordsOffset = piece.GetFootPrintOffset(this.BoardCoords);
                }
            }
            else if (tile != null )
            {
                var tr = tile.transform;

                //check if filter allows it
                if ( WhiteList == null || (WhiteList.Count != 0 && WhiteList.Contains(tr)))
                {
                    this.Selection = tr;
                }
            }
        }

        return this.Selection;
    }


    public Vector2Int GetCoords()
    {
        return this.BoardCoords;
    }

    public bool GetHover()
    {
        return this.BoardHover;
    }

    public Vector2Int GetOffset()
    {
        return this.BoardCoordsOffset;
    }
}
