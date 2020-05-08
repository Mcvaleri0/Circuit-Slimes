using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceSelectionResponse : MonoBehaviour, IBoardSelectionResponse
{
    //visual selection of the board space
    public GameObject BoardSpaceSelection;

    private SelectionManager Manager;

    //class used to contai the info about each space highlighted
    private class SelectionSpace {

        //Possible Colors
        public enum Colors
        {
            White,
            Red,
            Green
        };

        //Color Values
        private Dictionary<Colors, Color> ColorDict = new Dictionary<Colors, Color>()
        {
            { Colors.White, Color.white },
            { Colors.Red,   Color.HSVToRGB(0, 0.80f, 1) },
            { Colors.Green, Color.green },
        };

        //attributes
        private Vector2Int Coords;
        private Transform Transform;
        private Renderer Renderer;
        private MaterialPropertyBlock PropBlock;

        public SelectionSpace(Vector2Int _coords, Transform _transform, Renderer _renderer)
        {
            Coords = _coords;
            Transform = _transform;
            Renderer = _renderer;
            Renderer.enabled = false;

            PropBlock = new MaterialPropertyBlock();
        }

        //get transform
        public Transform GetTransform()
        {
            return Transform;
        }

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
            spacepos.y += 0.05f;
            Transform.position = spacepos;
        }

        //set color
        public void SetColor(Colors color)
        {
            var col = ColorDict[color]; 

            Renderer.GetPropertyBlock(PropBlock);
            PropBlock.SetColor("_Color", col);
            Renderer.SetPropertyBlock(PropBlock);
            
            Debug.Log(color);
        }
    }


    private readonly int NumSpaces = 4;

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


    //Update Selection
    public void UpdateSelection(Vector2Int boardCoords, bool boardHover, Transform selection, Vector2Int boardCoordsOffset) {

        // if we loose reference, re-init
        if (this.SelectionSpaces.Count != this.NumSpaces || this.SelectionSpaces[0].GetTransform() == null)
        {
            this.SelectionSpacesInit(this.NumSpaces);
        }

        //if nothing is selected just highlight the space under cursorn in white
        if (selection == null)
        {
            NoSelecionHighlight(boardCoords, boardHover);
        }
        //if we have something selected highight in the whole footprint, with colors
        else
        {
            FootprintHighlight(boardCoords, boardHover, selection, boardCoordsOffset);
        }
    }


    //set single space
    private void SingleSpaceHighlight(int i, Vector2Int coords, SelectionSpace.Colors color)
    {
        var space = SelectionSpaces[i];
        space.SetVisible(true);
        space.SetCoords(coords);
        space.SetColor(color);
    }


    //highlight one white space under cursor it no selection
    private void NoSelecionHighlight(Vector2Int boardCoords, bool boardHover) {

        //hide all
        this.SelectionSpaces.ForEach(e => e.SetVisible(false));

        //single white space
        if (boardHover)
        {
            SingleSpaceHighlight(0, boardCoords, SelectionSpace.Colors.White);
        }
    }


    //highlight single space in tile footprint (not used for now because at the moment tiles dont have footprints)
    private void TileHighlight(Vector2Int coords, Puzzle.Tile tile)
    {
        SelectionSpace.Colors color = SelectionSpace.Colors.White;

        var puzzle = this.Manager.PuzzleController.Puzzle;

        //if out of the board
        if (puzzle.OutOfBounds(coords) || !puzzle.IsFree(coords, tile))
        { 
            color = SelectionSpace.Colors.Red;
        }

        SingleSpaceHighlight(0, coords, color);
    }


    //highlight single space in footprint
    private void PieceFootprintHighlight(int i, Vector2Int boardCoords, Puzzle.Piece piece, Vector2Int offset = default)
    {
        SelectionSpace.Colors color = SelectionSpace.Colors.White;

        var puzzle = this.Manager.PuzzleController.Puzzle;

        var coords = boardCoords + offset;
        if (puzzle.OutOfBounds(coords))
        {
            color = SelectionSpace.Colors.Red;
        }
        else if(!puzzle.IsFree(boardCoords, piece))
        {
            var overlap = puzzle.GetPiece(coords);
            if (overlap != null && overlap != piece)
            {
                color = SelectionSpace.Colors.Red;
            }
        }

        SingleSpaceHighlight(i, coords, color);
    }


    //hightlight footprint
    private void FootprintHighlight(Vector2Int boardCoords, bool boardHover, Transform selection, Vector2Int boardCoordsOffset) {

        //check if piece or tile
        var piece = selection.GetComponent<Puzzle.Piece>();
        var tile = selection.GetComponent<Puzzle.Tile>();

        //adjust coords (because we might not be holding piece at the center of footprint)
        var adjustedCoords = boardCoords - boardCoordsOffset;

        if (piece != null)
        {
            //get footprint
            var footprint = piece.GetFootprintShape();
            var footprint_size = footprint.Length;

            var i = 0;
            foreach(SelectionSpace space in this.SelectionSpaces){
                
                if(i < footprint_size)
                {
                    this.PieceFootprintHighlight(i, adjustedCoords, piece, footprint[i]);

                    i++;
                    continue;
                }

                //if not needed for footprint, hide
                space.SetVisible(false);
            }
        }
        else if (tile != null)
        {
            //hide all
            this.SelectionSpaces.ForEach(e => e.SetVisible(false));

            this.TileHighlight(adjustedCoords, tile);
        }
    }
}
