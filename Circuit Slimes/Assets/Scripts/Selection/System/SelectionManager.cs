using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

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

    //white list of transforms that can be selected
    public List<Transform> WhiteList = null;

    //this signalizes wether we can change the seection right now or not
    public bool SelectionLocked = false;


    #region  === Initialization === 

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

    #region === Selection Locking/Unlocking Methods === 
    //input filter (one touch spot, simulated by mouse)
    private Lean.Touch.LeanFingerFilter InputFilter = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 1, 1, null);

    private bool IgnoreInput(Lean.Touch.LeanFinger finger)
    {
        //if input does not belong to filter
        if (!this.InputFilter.GetFingers().Contains(finger))
        {
            return true;
        }
        return false;
    }

    private void SelectionLock(Lean.Touch.LeanFinger finger)
    {
        if (this.IgnoreInput(finger)) return;

        //force update before locking
        this.Update();
        this.SelectionLocked = true;
    }

    private void SelectionUnlock(Lean.Touch.LeanFinger finger)
    {
        this.SelectionLocked = false;
    }

    #endregion

    #region === Unity Methods === 

    private void OnEnable()
    {
        //hook input down
        Lean.Touch.LeanTouch.OnFingerDown += this.SelectionLock;

        //hook input up
        Lean.Touch.LeanTouch.OnFingerUp += this.SelectionUnlock;

    }

    private void OnDisable()
    {
        //unhook input down
        Lean.Touch.LeanTouch.OnFingerDown -= this.SelectionLock;

        //unhook input up
        Lean.Touch.LeanTouch.OnFingerUp -= this.SelectionUnlock;
    }

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
        if (selection != this.CurrentSelection && !this.SelectionLocked)
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

    #region === Private Component Accessors === 

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