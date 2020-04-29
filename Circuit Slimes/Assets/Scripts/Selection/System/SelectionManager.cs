using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    private IRayProvider            RayProvider;
    private ITransformSelector      TransformSelector;
    private ICoordSelector          BoardCoordSelector;
    private IPieceSelectionResponse PieceSelectionResponse;
    private IBoardSelectionResponse BoardSelectionResponse;

    public Puzzle.PuzzleController PuzzleController { get; private set; }
    public Transform PuzzleTransform { get; private set; }

    public Transform BoardTransform { get; private set; }

    //Outward facing info, fed to the CreatorController
    public Transform CurrentSelection { get; private set; }
    public Vector2Int BoardCoords { get; private set; }
    public bool BoardHover { get; private set; }

    public List<Transform> WhiteList = null;



    #region Initialization

    public void Initialize(Puzzle.PuzzleController puzzleController, Transform puzzleObject)
    {
        //puzzle
        this.PuzzleController = puzzleController;
        this.PuzzleTransform = puzzleObject;

        //board
        this.BoardTransform = this.PuzzleTransform.Find("Board").transform;

        //ray provider, selectors and responses
        this.RayProvider            = this.GetComponent<IRayProvider>();
        this.TransformSelector      = this.GetComponent<ITransformSelector>();
        this.BoardCoordSelector     = this.GetComponent<ICoordSelector>();
        this.PieceSelectionResponse = this.GetComponent<IPieceSelectionResponse>();
        this.BoardSelectionResponse = this.GetComponent<IBoardSelectionResponse>();
    }

    public void ReInitialise(Puzzle.PuzzleController puzzleController)
    {
        //puzzle
        this.PuzzleController = puzzleController;
        this.PuzzleTransform = GameObject.Find("Puzzle").transform;

        //board
        this.BoardTransform = this.PuzzleTransform.Find("Board").transform;

    }

    #endregion


    private bool SelectionLocked()
    {
        //if holding mouse1 button or holding touch, lock selection
        if (Input.GetMouseButton(0) && 
            Lean.Touch.LeanTouch.Fingers.Count > 0 && 
            this.CurrentSelection != null)
        {
            return true;
        }
        return false;
    }

    #region Unity Methods

    private void Update()
    {
        if (this.BoardTransform == null) {
            ReInitialise(this.PuzzleController);
        }

        //new ray
        var ray = this.RayProvider.CreateRay();

        //Get Coords from board
        this.BoardCoords = this.GetBoardCoords(ray);
        this.BoardHover = this.GetBoardHover();

        //Get Selection
        var selection = GetSelection(ray);

        //Piece Selection Response
        if (selection != this.CurrentSelection && !this.SelectionLocked())
        {
            //Deselection Response
            if (this.CurrentSelection != null)
            {
                this.PieceSelectionResponse.OnDeselect(this.CurrentSelection);
            }

            this.CurrentSelection = selection;

            //Selection Response
            if (this.CurrentSelection != null)
            {
                this.PieceSelectionResponse.OnSelect(this.CurrentSelection);
            }
        }

        //Board Selection Response
        this.BoardSelectionResponse.UpdateSelection(this.BoardCoords, this.BoardHover, this.CurrentSelection);
      
        //Debug.Log(CurrentSelection);
        //Debug.Log(BoardCoords);
        //Debug.Log(BoardHover);
    }

    #endregion


    #region Private Component Accessors

    private Transform GetSelection(Ray ray)
    {
        return this.TransformSelector.Check(ray);
    }

    private Vector2Int GetBoardCoords(Ray ray)
    {
        return this.BoardCoordSelector.GetCoords(ray);
    }

    private bool GetBoardHover()
    {
        return this.BoardCoordSelector.GetHover();
    }

    #endregion
    }