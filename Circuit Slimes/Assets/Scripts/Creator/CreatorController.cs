using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.UI;
using Creator.Mode;
using Creator.Editor;
using Creator.Selection;



namespace Creator
{
    public class CreatorController : MonoBehaviour
    {
        #region /* UI Atributes */

        private UIController UIController { get; set; }

        #endregion


        #region /* Puzzle Atributes */

        private PuzzleController PuzzleController { get; set; }

        private PuzzleEditor PuzzleEditor { get; set; }

        #endregion


        #region /* Selection Atributes */

        private SelectionSystem SelectionSystem { get; set; }

        #endregion


        #region /* Mode Atributes */

        private Mode.Mode Mode { get; set; }

        public bool Creator { get; private set; }

        public Dictionary<Vector2Int, Piece.Caracteristics> PiecesAdded { get; private set; }

        #endregion



        #region === Input Events ===

        private Lean.Touch.LeanFingerFilter InputFilter = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 1, 1, null);


        private bool IgnoreInput(Lean.Touch.LeanFinger finger)
        {
            //if input does not belong to filter
            if (!this.InputFilter.GetFingers().Contains(finger))
            {
                return true;
            }
            //if input is over gui or multiple fingers are used, ignore
            if (finger.StartedOverGui || Lean.Touch.LeanTouch.Fingers.Count > 1) {
                return true;
            }
            return false;
        }

    
        private void OnInputDown(Lean.Touch.LeanFinger finger)
        {
            if (this.IgnoreInput(finger)) return;

            //if we have an item to place, place it
            if (this.PuzzleEditor.HasItemToPlace())
            {
                this.PuzzleEditor.PlaceItem();
            }

            //if double click delete
            else if (this.SelectionSystem.DoubleClick())
            {
                this.PuzzleEditor.RemoveItem();
            }
        }


        private void OnInputUp(Lean.Touch.LeanFinger finger)
        {
            //ignore finger up that happened over the ui
            if (finger.StartedOverGui) { return; }

            // If we have an item to place then it must be placed
            if (!this.PuzzleEditor.HasItemToPlace())
            {
                this.SelectionSystem.EndDrag();
            }
        }


        private void OnInputDrag(Lean.Touch.LeanFinger finger)
        {
            if (this.IgnoreInput(finger)) return;

            // If we have an item to place then it must be placed
            if (!this.PuzzleEditor.HasItemToPlace())
            {
                //Drag Item
                if (this.SelectionSystem.Dragging)
                {
                    this.PuzzleEditor.MoveItem();
                }
                //if something is selected prepare drag
                else if (this.SelectionSystem.SomethingSelected())
                {
                    this.SelectionSystem.PrepareDrag();
                }
            }
        }

        #endregion


        #region === Unity Events ===

        private void OnEnable()
        {
            //hook input down
            Lean.Touch.LeanTouch.OnFingerDown += this.OnInputDown;

            //hook input up
            Lean.Touch.LeanTouch.OnFingerUp += this.OnInputUp;

            //hook input drag
            Lean.Touch.LeanTouch.OnFingerSet += this.OnInputDrag;
        }

        private void OnDisable()
        {
            //unhook input down
            Lean.Touch.LeanTouch.OnFingerDown -= this.OnInputDown;

            //unhook input up
            Lean.Touch.LeanTouch.OnFingerUp -= this.OnInputUp;

            //unhook input drag
            Lean.Touch.LeanTouch.OnFingerSet -= this.OnInputDrag;
        }

        #endregion


        #region === Init/Update Methods ===

        public void Initialize(PuzzleController controller, Puzzle.Puzzle puzzle, bool creator)
        {
            this.InitializePuzzleInfo(controller, puzzle);

            this.InitializeSelectionSystem();

            this.InitializePlayerCreatorMode(creator);

            this.InitializeUI();
        }

        public void UpdateInfo(Puzzle.Puzzle puzzle)
        {
            this.PuzzleEditor.UpdatePuzzle(puzzle);

            this.SelectionSystem.UpdateInfo();

            this.InitializePlayerCreatorMode(this.Creator);

            this.UIController.UpdateUI();
        }

        #endregion


        #region === Puzzle Methods ===

        private void InitializePuzzleInfo(PuzzleController controller, Puzzle.Puzzle puzzle)
        {
            this.PuzzleController = controller;

            this.PuzzleEditor = new PuzzleEditor(puzzle);
        }

        #endregion


        #region === Selection System Methods ===

        private void InitializeSelectionSystem()
        {
            SelectionManager manager = this.transform.Find("SelectionManager").GetComponent<SelectionManager>();

            this.SelectionSystem = new SelectionSystem(this.PuzzleEditor, manager, this.PuzzleController);
        }

        #endregion


        #region === Player/Creator Methods ===

        private void InitializePlayerCreatorMode(bool creator)
        {
            if (creator)
            {
                this.Mode = new Mode.Editor(this.SelectionSystem);
            }
            else
            {
                this.Mode = new Player(this.SelectionSystem);
            }

            this.Mode.DefineSelectableList();

            this.PiecesAdded = new Dictionary<Vector2Int, Piece.Caracteristics>();
        }

        #endregion


        #region === UI Methods ===

        private void InitializeUI()
        {
            Transform canvas = this.transform.Find("Canvas");

            this.UIController = new UIController(this.PuzzleEditor, this.SelectionSystem,
                                        this.Mode, this.PuzzleController, canvas);
        }

        #endregion

    }
}
