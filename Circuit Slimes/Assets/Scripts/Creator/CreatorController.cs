using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Puzzle;



namespace Creator
{
    public class CreatorController : MonoBehaviour
    {
        #region /* UI Atributes */

        private ScrollMenu ScrollMenu { get; set; }
        private Transform  SaveButton { get; set; }

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

        private Piece PieceSelected { get; set; }
        private Tile TileSelected { get; set; }

        private bool MouseHolded { get; set; }
        private Vector3 PosInScreenSpace { get; set; }
        private Vector3 Offset { get; set; }

        #endregion


        #region /* Player/Creator Mode Atributes */

        private const string ITEMS_PATH = "Prefabs/Board Items";

        public bool Creator;

        public List<Piece> PiecesAdded { get; private set; }

        private List<string> PrefabsAllowed { get; set; }

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

            this.InitializePlayerCreatorMode();

            this.InitializeCanvas();

            this.InitializeSelectionSystem();
        }

        private void InitializePuzzle()
        {
            this.PuzzleController = GameObject.Find("PuzzleController").GetComponent<PuzzleController>();

            if (this.Creator)
            {
                // TODO: Choose Level
                this.Puzzle = this.PuzzleController.LoadPuzzle(this.PuzzleController.CurrentLevel);
            }
            else
            {
                this.Puzzle = this.PuzzleController.LoadPuzzle(this.PuzzleController.CurrentLevel);
            }

            this.PuzzleObj = GameObject.Find("Puzzle").transform;
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

        private void InitializePlayerCreatorMode()
        {
            if (!this.Creator)
            {
                this.PrefabsAllowed = this.Puzzle.Permissions;
            }
            else
            {
                Object[] prefabs = Resources.LoadAll(ITEMS_PATH);
                this.PrefabsAllowed = new List<string>();

                foreach (Object prefab in prefabs)
                {
                    this.PrefabsAllowed.Add(prefab.name);
                }
            }


            this.PiecesAdded = new List<Piece>();
        }

        #region = Initialization Aux Methods =

        private void InitializeScrollMenu(Transform canvas)
        {
            Transform menu = canvas.Find("Scroll Menu");
            Transform content = menu.Find("Viewport").Find("Content");

            this.ScrollMenu = new ScrollMenu(this, menu, content, this.PrefabsAllowed);
        }

        private void InitializeButtons(Transform canvas)
        {
            // Change SaveButton Location
            this.SaveButton = canvas.Find("Save Button");

            RectTransform saveRect = this.SaveButton.GetComponent<RectTransform>();

            float x = (Screen.width / 2) - (saveRect.sizeDelta.x / 2) - 5;
            float y = (Screen.height / 2) - (saveRect.sizeDelta.y / 2) - 5;
            saveRect.anchoredPosition = new Vector2(x, y);

            // add click listener
            int level = this.PuzzleController.CurrentLevel;
            this.SaveButton.GetComponent<Button>().onClick.AddListener(delegate { this.PuzzleController.SavePuzzle(level); });

            if (!this.Creator)
            {
                this.SaveButton.gameObject.SetActive(false);
            }
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

                // one of them is always null
                this.PieceSelected = this.Selected.GetComponent<Piece>();
                this.TileSelected  = this.Selected.GetComponent<Tile>();

                //reset and disable tile temporarily (visual) 
                if (this.TileSelected != null)
                {
                    //this.TileSelected.UpdateTile(0); 
                    this.TileSelected.enabled = false;
                }
                
            }
        }

        private void EndDrag()
        {
            this.MouseHolded = false;

            if (this.Selected != null)
            {
                Vector2Int newPos = this.Puzzle.Discretize(this.Selected.position);

                if (this.PieceSelected != null)
                {
                    this.Puzzle.MovePiece(newPos, this.PieceSelected);
                }
                else
                {
                    this.Puzzle.MoveTile(newPos, this.TileSelected);

                    //re-enable tile  (visual)
                    this.TileSelected.enabled = true;
                }
            }
        }

        #endregion


        #region === Puzzle Manipulation Methods ===

        public void AddBoardItem(string name)
        {
            Debug.Log("Instantiating " + name);

            // TODO: make the player choose where he wants the item
            Vector2Int coords = new Vector2Int(0, 3);

            if (name.Contains("Tile"))
            {
                Tile newTile = Tile.CreateTile(this.Puzzle, coords, name);
                this.Puzzle.AddTile(newTile);
            }
            else
            {
                Piece newPiece = Piece.CreatePiece(this.Puzzle, coords, name);
                this.Puzzle.AddPiece(newPiece);
                this.PiecesAdded.Add(newPiece);
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
            //get board coords
            Vector2Int coords = this.SelectionManager.BoardCoords;

            //convert grid coords in world coords
            Vector3 curPosition = this.Puzzle.WorldCoords(coords) + Offset;

            // the new position must be at the board surface
            curPosition = this.Puzzle.AtBoardSurface(curPosition);
            Piece pieceNewPos = this.Puzzle.GetPiece(coords);
            Tile tileNewPos   = this.Puzzle.GetTile(coords);

            // update the position of the object in the world
            if ((this.PieceSelected != null && (this.Creator || pieceNewPos == null)) ||
                (this.TileSelected  != null && tileNewPos  == null))
            {
                this.Selected.position = curPosition;
            }
        }

        #endregion

    }
}
