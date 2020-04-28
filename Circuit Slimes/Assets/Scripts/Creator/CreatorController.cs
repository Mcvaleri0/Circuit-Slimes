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



        #region === Unity Events ===

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (this.SelectionSystem.DoubleClick())
                {
                    this.PuzzleEditor.RemoveItem();
                }
                else if (this.SelectionSystem.SomethingSelected())
                {
                    this.SelectionSystem.PrepareDrag();
                }
                else if (this.PuzzleEditor.HasItemToPlace())
                {
                    this.PuzzleEditor.PlaceItem();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.SelectionSystem.EndDrag();
            }

            if (this.SelectionSystem.MouseHolded)
            {
                this.PuzzleEditor.MoveItem();
            }
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
