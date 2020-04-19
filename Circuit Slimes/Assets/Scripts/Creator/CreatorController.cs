using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Puzzle;
using Puzzle.Board;



namespace Creator
{
    public class CreatorController : MonoBehaviour
    {
        #region /* UI Atributes */

        private ScrollMenu ScrollMenu { get; set; }

        #endregion

        #region /* Puzzle Atributes */

        private PuzzleController PuzzleController { get; set; }
        private Puzzle.Puzzle Puzzle { get; set; }
        private Transform PuzzleObj { get; set; }

        #endregion

        #region /* Selection Atributes */

        private const float DOUBLE_CLICK_WINDOW = 0.5f;

        private bool SingleClick { get; set; }
        private float TimeFirstClick { get; set; }

        private SelectionManager SelectionManager { get; set; }
        private Transform Selected { get; set; }
        private bool MouseHolded { get; set; }
        private Vector3 PosInScreenSpace { get; set; }
        private Vector3 Offset { get; set; }

        #endregion


        #region === Unity Events ===

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (this.DoubleClick())
                {
                    this.RemoveBoardItem();
                }

                this.PrepareDrag();
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.EndDrag();
            }

            if (MouseHolded)
            {
                this.MoveBoardItem();
            }
        }

        #endregion

        #region === Initialization Methods ===

        public void Initialize()
        {
            this.InitializePuzzle();

            this.InitializeCanvas();

            this.InitializeSelectionSystem();
        }

        private void InitializePuzzle()
        {
            this.PuzzleController = GameObject.Find("PuzzleController").GetComponent<PuzzleController>();
            this.PuzzleObj = GameObject.Find("Puzzle").transform;

            this.Puzzle = this.PuzzleController.Puzzle;
        }

        private void InitializeCanvas()
        {
            Transform canvas = this.transform.Find("Canvas");

            this.InitializeScrollMenu(canvas);

            this.InitializeButtons(canvas);
        }

        private void InitializeSelectionSystem()
        {
            this.SingleClick = false;
            this.SelectionManager = this.transform.Find("SelectionManager").GetComponent<SelectionManager>();

            this.SelectionManager.Initialize(this.PuzzleController, this.PuzzleObj);
        }

        #region = Initialization Aux Methods =

        private void InitializeScrollMenu(Transform canvas)
        {
            Transform menu = canvas.Find("Scroll Menu");
            Transform content = menu.Find("Viewport").Find("Content");

            this.ScrollMenu = new ScrollMenu(this, menu, content);
        }

        private void InitializeButtons(Transform canvas)
        {
            // Change SaveButton Location
            Transform save = canvas.Find("Save Button");
            RectTransform saveRect = save.GetComponent<RectTransform>();

            float x = (Screen.width  / 2) - (saveRect.sizeDelta.x / 2) - 5;
            float y = (Screen.height / 2) - (saveRect.sizeDelta.y / 2) - 5;
            saveRect.anchoredPosition = new Vector2(x, y);

            // add click listener
            int level = this.PuzzleController.CurrentLevel;
            save.GetComponent<Button>().onClick.AddListener(delegate { this.PuzzleController.SavePuzzle(level); });
        }

        #endregion

        #endregion

        #region === Selection Methods ===

        private bool DoubleClick()
        {
            if (!this.SingleClick)
            {
                this.TimeFirstClick = Time.time;
                this.SingleClick    = true;
            }
            else
            {
                if ((Time.time - this.TimeFirstClick) > DOUBLE_CLICK_WINDOW)
                {
                    this.TimeFirstClick = Time.time;
                }
                else
                {
                    this.SingleClick = false;
                    return true;
                }
            }

            return false;
        }

        private void PrepareDrag()
        {
            this.Selected = this.SelectionManager.CurrentSelection;

            if (this.Selected != null)
            {
                this.MouseHolded = true;
                this.PosInScreenSpace = Camera.main.WorldToScreenPoint(this.Selected.position);

                Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, PosInScreenSpace.z);
                this.Offset = this.Selected.position - Camera.main.ScreenToWorldPoint(newPosition);
            }
        }

        private void EndDrag()
        {
            MouseHolded = false;
        }

        #endregion

        #region === Puzzle Manipulation Methods ===

        public void AddBoardItem(string name)
        {
            Debug.Log("Instantiating " + name);

            // TODO: make the player choose where he wants the item
            Vector2Int coords = new Vector2Int(0, 3);

            Transform parent;
            if (name.Contains("Tile"))
            {
                parent = this.PuzzleObj.Find("Tiles");
                Tile newTile = Tile.CreateTile(parent, this.Puzzle, coords, name);
                this.Puzzle.AddTile(newTile);
            }
            else
            {
                parent = this.PuzzleObj.Find("Pieces");
                Piece newPiece = Piece.CreatePiece(parent, this.Puzzle, coords, name);
                this.Puzzle.AddPiece(newPiece);
            }
        }

        private void RemoveBoardItem()
        {
            if (this.SelectionManager.CurrentSelection != null)
            {
                GameObject objToRemove = this.SelectionManager.CurrentSelection.gameObject;
                Vector2Int coords = this.SelectionManager.BoardCoords;

                // remove object representation from Puzzle
                Piece pieceToRemove = this.Puzzle.GetPiece(coords);
                Tile  tileToRemove  = this.Puzzle.GetTile(coords);

                if (pieceToRemove != null)
                {
                    this.Puzzle.RemovePiece(pieceToRemove);
                    GameObject.Destroy(objToRemove);
                }
                else if (tileToRemove != null)
                {
                    this.Puzzle.RemoveTile(tileToRemove);
                    GameObject.Destroy(objToRemove);
                }
            }
        }

        private void MoveBoardItem()
        {
            //keep track of the mouse position
            var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, PosInScreenSpace.z);

            //convert the screen mouse position to world point and adjust with offset
            var curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + Offset;

            //update the position of the object in the world
            this.Selected.position = curPosition;

            // TODO: position must be at the board's surface 
        }

        #endregion

    }
}
