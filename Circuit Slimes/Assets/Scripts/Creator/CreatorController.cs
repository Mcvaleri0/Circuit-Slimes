﻿using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game;
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

        public GameController PuzzleController { get; set; }

        private PuzzleEditor PuzzleEditor { get; set; }

        #endregion


        #region /* Selection Atributes */

        private SelectionSystem SelectionSystem { get; set; }

        #endregion


        #region /* Mode Atributes */

        private Mode.Mode Mode { get; set; }

        public bool Creator { get; private set; }

        #endregion


        #region /* Analytics Attributes */
        
        public bool saveAnalytics { get; set; }
        
        #endregion



        #region === Input Events ===

        private Lean.Touch.LeanFingerFilter InputFilter = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 1, 1, null);
        private Lean.Touch.LeanFingerFilter RightFilter = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 1, 2, null);


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
            //if (this.IgnoreInput(finger)) return;

            if (this.RightFilter.GetFingers().Contains(finger))
            {
                this.PuzzleEditor.RotateItem();
            }

            //if double click delete
            //if (Input.GetKeyDown(KeyCode.Mouse1))
            ////if (this.SelectionSystem.DoubleClick())
            //{
            //}
            //else if (!this.SelectionSystem.Dragging && this.SelectionSystem.SomethingSelected())
            //{
            //    this.SelectionSystem.PrepareDrag();
            //}

        }


        private void OnInputUp(Lean.Touch.LeanFinger finger)
        {
            //ignore finger up that happened over the ui
            //if (finger.StartedOverGui) { return; }

            if (this.SelectionSystem.Dragging)
            {
                this.SelectionSystem.EndDrag();
            }
        }


        private void OnInputDrag(Lean.Touch.LeanFinger finger)
        {
            if (this.IgnoreInput(finger)) return;

            //Drag Item
            //if (this.SelectionSystem.Dragging)
            //{
            //    this.PuzzleEditor.MoveItem();
            //}
            //if something is selected prepare drag
            //else if (this.SelectionSystem.SomethingSelected())
            //if (this.SelectionSystem.SomethingSelected())
            //{
            //    this.SelectionSystem.PrepareDrag();
            //}
            if (!this.SelectionSystem.Dragging && this.SelectionSystem.SomethingSelected())
            {
                this.SelectionSystem.PrepareDrag();
            }

        }

        #endregion


        #region === Unity Events ===

        private void Awake()
        {
            this.PuzzleController = GameController.CreateGameController();
        }


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


        #region === Controller Methods ===

        public void Initialize(Puzzle.Puzzle puzzle, bool creator)
        {
            this.InitializePuzzleInfo(puzzle);

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

        private void InitializePuzzleInfo(Puzzle.Puzzle puzzle)
        {
            this.PuzzleEditor = new PuzzleEditor(this, this.PuzzleController.AnalyticsController, puzzle);
        }


        public void RemoveItemsPlaced()
        {
            this.PuzzleEditor.RemoveItemsPlaced();
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

            this.saveAnalytics = creator;

            this.PuzzleEditor.Mode = this.Mode;

            this.Mode.DefineSelectableList();
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
