using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

using Level;



public class SelectionManager : MonoBehaviour
{

    private IRayProvider            RayProvider;
    private ITransformSelector      TransformSelector;
    private ICoordSelector          BoardCoordSelector;
    private IPieceSelectionResponse PieceSelectionResponse;
    private IBoardSelectionResponse BoardSelectionResponse;

    public LevelController PuzzleController { get; private set; }
    public Transform PuzzleTransform { get; private set; }

    public Transform BoardTransform { get; private set; }

    //Outward facing info, fed to the CreatorController
    private Transform CurrentSelection { get; set; }
    private Vector2Int BoardCoords { get; set; }
    private bool BoardHover { get; set; }

    //white list of transforms that can be selected
    public List<Transform> WhiteList = null;

    //this signalizes wether we can change the seection right now or not
    public bool SelectionLocked = false;


    #region  === Initialization === 

    public void Initialize(LevelController puzzleController)
    {
        //puzzle
        this.PuzzleController = puzzleController;
        this.PuzzleTransform = GameObject.Find("Puzzle").transform;

        //board
        this.BoardTransform = this.PuzzleTransform.Find("Board").transform;

        //ray provider, selectors and responses
        this.RayProvider            = this.GetComponent<IRayProvider>();
        this.TransformSelector      = this.GetComponent<ITransformSelector>();
        this.BoardCoordSelector     = this.GetComponent<ICoordSelector>();
        this.PieceSelectionResponse = this.GetComponent<IPieceSelectionResponse>();
        this.BoardSelectionResponse = this.GetComponent<IBoardSelectionResponse>();
    }

    public void ReInitialise(LevelController puzzleController)
    {
        //puzzle
        this.PuzzleController = puzzleController;
        this.PuzzleTransform = GameObject.Find("Puzzle").transform;

        //board
        this.BoardTransform = this.PuzzleTransform.Find("Board").transform;

    }

    #endregion

    #region === Public Accessor Methods ===

    public Vector2Int GetBoardCoords()
    {
        this.Update();
        return this.BoardCoords;
    }

    public bool GetBoardHover()
    {
        this.Update();
        return this.BoardHover;
    }

    public Transform GetCurrentSelection()
    {
        this.Update();
        return this.CurrentSelection;
    }

    #endregion

    #region === Input Methods === 
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
        //re-init if reference is lost
        if (this.BoardTransform == null) {
            this.ReInitialise(this.PuzzleController);
        }

        //new ray
        var ray = this.RayProvider.CreateRay();

        //Get Coords from CoordSelector
        this.BoardCoords = this.BoardCoordSelector.GetCoords(ray);

        //Get Hover from CoordSelector
        this.BoardHover = this.BoardCoordSelector.GetHover();

        //Get Selection from TransformSelector
        var selection = this.TransformSelector.Check(ray);

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

        //Debug 
        //this.PrintAttributes();
    }

    #endregion

    #region === Debug Methods ===

    public void PrintAttributes()
    {
        var selection = (CurrentSelection == null) ? "nothing" : CurrentSelection.name;
        Debug.Log("Selection { Transform: " + selection + ", Coords: " + BoardCoords + ", Hover: " + BoardHover + "}");
    }

    #endregion
}