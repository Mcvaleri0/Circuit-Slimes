using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceSelectionResponse : MonoBehaviour, IBoardSelectionResponse
{
    //visual selection of the board space
    public GameObject BoardSpaceSelection;
    private Transform BoardSpaceSelectionTransform;
    private MeshRenderer BoardSpaceSelectionRenderer;

    private SelectionManager Manager;

    //class used to contai the info about each space highlighted
    private class SelectionSpace {

        public enum Color { White, Red, Green};

        public SelectionSpace(Vector2Int _coords, Transform _transform, Renderer _renderer)
        {
            Coords = _coords;
            Transform = _transform;
            Renderer = _renderer;
            Renderer.enabled = false;
        }

        public Vector2Int Coords;
        public Transform Transform;
        public Renderer Renderer;

        //set visibility
        public void SetVisible(bool b)
        {
            Renderer.enabled = b;
        }

        //set coords
        public void SetCoords(Vector2Int coords)
        {
            Coords = coords;

            var spacepos = Puzzle.Board.LevelBoard.WorldCoords(Coords);
            spacepos.y += 0.1f;
            Transform.position = spacepos;
        }

        //set color
        public void SetColor(Color color)
        {
            //FIXME: swap mat
        }
    }

    private int NumSpaces = 4;

    private List<SelectionSpace> SelectionSpaces = new List<SelectionSpace>();


    //initialize the selection spaces that will be used
    public void SelectionSpacesInit(int numSpaces)
    {
        SelectionSpaces.Clear();

        for (var i = 0; i < numSpaces; i++)
        {

            //if no board is present abort
            if(GameObject.Find("Puzzle") == null) { return; }

            //create object
            var boardSpaceSelection = Instantiate(BoardSpaceSelection, this.Manager.PuzzleTransform);

            //get transform and renderer
            var transform = boardSpaceSelection.transform;
            var renderer = boardSpaceSelection.GetComponent<MeshRenderer>();

            //save in struct
            SelectionSpaces.Add(new SelectionSpace(new Vector2Int(0,0), transform, renderer));
        }
    }

    public void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();

        this.SelectionSpacesInit(this.NumSpaces);
    }

    public void UpdateSelection(Vector2Int boardCoords, bool boardHover, Transform selection) {

        // if we loose reference, re-init
        if (this.SelectionSpaces.Count != this.NumSpaces || this.SelectionSpaces[0].Transform == null)
        {
            this.SelectionSpacesInit(this.NumSpaces);
        }

        //if nothing is selected just highlight the space under cursorn in white
        if (selection == null)
        {
            SingleSpaceHighlight(boardCoords, boardHover);
        }
        //if we have something selected highight in the whole footprint, with colors
        else
        {
            FootPrintHighlight(boardCoords, boardHover, selection);
        }
    }


    private void SingleSpaceHighlight(Vector2Int boardCoords, bool boardHover) {

        //hide all
        SelectionSpaces.ForEach(e => e.SetVisible(false));

        //set single space
        var space = SelectionSpaces[0];
        space.SetVisible(boardHover);
        space.SetCoords(boardCoords);
        //space.SetColor(SelectionSpace.Color.White);
    }

    private void FootPrintHighlight(Vector2Int boardCoords, bool boardHover, Transform selection) {

        //get number of spaces needed
        SelectionSpaces.ForEach(e => e.SetVisible(false));

        //FIX ME: here we should check and highlight footprint
    }
}
