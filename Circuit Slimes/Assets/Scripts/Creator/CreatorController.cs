using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Mode;
using Creator.Editor;
using Creator.Selection;



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

        private PuzzleEditor PuzzleEditor { get; set; }

        #endregion


        #region /* Selection Atributes */

        private SelectionManager SelectionManager { get; set; }
        private SelectionSystem SelectionSystem { get; set; }

        #endregion


        #region /* Mode Atributes */

        private const string ITEMS_PATH = "Prefabs/Board Items";

        private Mode.Mode Mode { get; set; }

        public bool Creator { get; private set; }

        public Dictionary<Vector2Int, Piece.Caracteristics> PiecesAdded { get; private set; }

        private List<string> MenuOptions { get; set; }

        #endregion  



        #region === Unity Events ===

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                if (this.SelectionSystem.DoubleClick())
                {
                    this.RemoveBoardItem();
                }

                this.SelectionSystem.PrepareDrag();
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

            this.InitializeCanvas();
        }

        public void UpdateInfo(Puzzle.Puzzle puzzle)
        {
            this.UpdatePuzzle(puzzle);

            //this.UpdateSelectionSystem();
            this.SelectionSystem.UpdateInfo(this.PuzzleController);

            this.InitializePlayerCreatorMode(this.Creator);

            this.UpdateCanvas();
        }

        #endregion


        #region === Puzzle Methods ===

        private void InitializePuzzleInfo(PuzzleController controller, Puzzle.Puzzle puzzle)
        {
            this.PuzzleController = controller;
            this.Puzzle = puzzle;
            this.PuzzleObj = puzzle.transform;

            this.PuzzleEditor = new PuzzleEditor(puzzle);
        }

        private void UpdatePuzzle(Puzzle.Puzzle puzzle)
        {
            this.Puzzle = puzzle;
            this.PuzzleObj = this.Puzzle.transform;
        }

        public void AddBoardItem(string name)
        {
            Debug.Log("Instantiating " + name);

            // TODO: make the player choose where he wants the item
            Vector2Int coords = new Vector2Int(0, 3);

            if (name.Contains("Tile"))
            {
                Tile newTile = Tile.CreateTile(this.Puzzle, coords, name);
                this.Puzzle.AddTile(newTile);
                this.SelectionManager.WhiteList.Add(newTile.transform);
            }
            else
            {
                Piece newPiece = Piece.CreatePiece(this.Puzzle, coords, name);
                this.Puzzle.AddPiece(newPiece);
                this.SelectionManager.WhiteList.Add(newPiece.transform);
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
                Tile tileToRemove = this.Puzzle.GetTile(coords);

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

        //private void MoveBoardItem()
        //{
        //    //get board coords
        //    Vector2Int coords = this.SelectionManager.BoardCoords;

        //    //convert grid coords in world coords
        //    Vector3 curPosition = this.Puzzle.WorldCoords(coords) + Offset;

        //    // the new position must be at the board surface
        //    curPosition = this.Puzzle.AtBoardSurface(curPosition);
        //    Piece pieceNewPos = this.Puzzle.GetPiece(coords);
        //    Tile  tileNewPos  = this.Puzzle.GetTile(coords);

        //    // update the position of the object in the world
        //    if ((this.PieceSelected != null && (pieceNewPos == null || pieceNewPos == this.PieceSelected)) ||
        //        (this.TileSelected  != null && (tileNewPos  == null || tileNewPos  == this.TileSelected )))
        //    {
        //        this.Selected.position = curPosition;
        //    }
        //}

        public void AddPermission(string prefab)
        {
            this.Puzzle.Permissions.Add(prefab);
        }

        public void RemovePermission(string prefab)
        {
            this.Puzzle.Permissions.Remove(prefab);
        }

        #endregion


        #region === Selection System Methods ===

        private void InitializeSelectionSystem()
        {
            //this.SingleClick = false;

            SelectionManager manager = this.transform.Find("SelectionManager").GetComponent<SelectionManager>();
            //this.SelectionManager.Initialize(this.PuzzleController, this.PuzzleObj);

            this.SelectionSystem = new SelectionSystem(manager, this.PuzzleController, this.PuzzleEditor);

            this.PuzzleEditor.Selection = this.SelectionSystem;
        }

        //private void InitializeWhiteListCreator()
        //{
        //    this.SelectionManager.WhiteList = new List<Transform>();

        //    foreach (Transform childPiece in this.Puzzle.PiecesObj.transform)
        //    {
        //        this.SelectionManager.WhiteList.Add(childPiece);
        //    }

        //    foreach (Transform childTile in this.Puzzle.TilesObj.transform)
        //    {
        //        this.SelectionManager.WhiteList.Add(childTile);
        //    }
        //}

        //private void UpdateSelectionSystem()
        //{
        //    this.SingleClick = false;

        //    this.SelectionManager.ReInitialise(this.PuzzleController);
        //}

        //private bool DoubleClick()
        //{
        //    if (!this.SingleClick)
        //    {
        //        this.TimeFirstClick = Time.time;
        //        this.SingleClick = true;
        //    }
        //    else
        //    {
        //        if ((Time.time - this.TimeFirstClick) > DOUBLE_CLICK_WINDOW)
        //        {
        //            this.TimeFirstClick = Time.time;
        //        }
        //        else
        //        {
        //            this.SingleClick = false;
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private void PrepareDrag()
        //{
        //    this.Selected = this.SelectionManager.CurrentSelection;

        //    if (this.Selected != null)
        //    {
        //        this.MouseHolded = true;
        //        this.PosInScreenSpace = Camera.main.WorldToScreenPoint(this.Selected.position);

        //        Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, PosInScreenSpace.z);
        //        this.Offset = this.Selected.position - Camera.main.ScreenToWorldPoint(newPosition);

        //        // one of them is always null
        //        this.PieceSelected = this.Selected.GetComponent<Piece>();
        //        this.TileSelected = this.Selected.GetComponent<Tile>();

        //        //reset and disable tile temporarily (visual) 
        //        if (this.TileSelected != null)
        //        {
        //            //this.TileSelected.UpdateTile(0); 
        //            this.TileSelected.enabled = false;
        //        }

        //    }
        //}

        //private void EndDrag()
        //{
        //    this.MouseHolded = false;

        //    if (this.Selected != null)
        //    {
        //        Vector2Int newPos = this.Puzzle.Discretize(this.Selected.position);

        //        if (this.PieceSelected != null)
        //        {
        //            this.Puzzle.MovePiece(newPos, this.PieceSelected);
        //        }
        //        else
        //        {
        //            this.Puzzle.MoveTile(newPos, this.TileSelected);

        //            //re-enable tile  (visual)
        //            this.TileSelected.enabled = true;
        //        }
        //    }
        //}

        #endregion


        #region === Player/Creator Methods ===

        private void InitializePlayerCreatorMode(bool creator)
        {
            this.Creator = creator;

            if (this.Creator)
            {
                this.InitializeMenuCreator();

                this.SelectionSystem.WhiteListAllItens();
                //this.InitializeWhiteListCreator();
            }
            else
            {
                this.MenuOptions = this.Puzzle.Permissions;

                this.SelectionManager.WhiteList = new List<Transform>();
            }

            this.PiecesAdded = new Dictionary<Vector2Int, Piece.Caracteristics>();
        }

        #endregion


        #region === UI Methods ===

        private void InitializeCanvas()
        {
            Transform canvas = this.transform.Find("Canvas");

            this.InitializeScrollMenu(canvas);

            this.InitializeButtons(canvas);
        }

        private void InitializeMenuCreator()
        {
            if (this.MenuOptions == null)
            {
                Object[] prefabs = Resources.LoadAll(ITEMS_PATH);
                this.MenuOptions = new List<string>();

                foreach (Object prefab in prefabs)
                {
                    this.MenuOptions.Add(prefab.name);
                }
            }
        }

        private void InitializeScrollMenu(Transform canvas)
        {
            Transform menu = canvas.Find("Scroll Menu");
            Transform content = menu.Find("Viewport").Find("Content");

            this.ScrollMenu = new ScrollMenu(this, menu, content, this.MenuOptions, this.Puzzle.Permissions);
        }

        private void InitializeButtons(Transform canvas)
        {
            // Change SaveButton Location
            this.SaveButton = canvas.Find("Save Button");

            if (this.Creator)
            {
                RectTransform saveRect = this.SaveButton.GetComponent<RectTransform>();

                saveRect.pivot = new Vector2(1, 1);
                float x = -30; //x margin
                float y = -30; //y margin

                saveRect.anchoredPosition = new Vector2(x, y);

                // add click listener
                int level = this.PuzzleController.CurrentLevel;
                this.SaveButton.GetComponent<Button>().onClick.AddListener(delegate { this.PuzzleController.SaveLevel(level); });
            }
            else
            {
                this.SaveButton.gameObject.SetActive(false);
            }
        }

        private void UpdateCanvas()
        {
            this.ScrollMenu.UpdateContent(this.MenuOptions, this.Puzzle.Permissions);
        }

        #endregion

    }
}
