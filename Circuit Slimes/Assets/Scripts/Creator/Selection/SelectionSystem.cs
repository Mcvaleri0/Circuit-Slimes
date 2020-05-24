using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;
using Puzzle;
using Creator.Editor;



namespace Creator.Selection
{
    public class SelectionSystem
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private GameController Controller { get; set; }

        #endregion


        #region /* Click Attributes */

        private const float DOUBLE_CLICK_WINDOW = 0.25f;

        private bool SingleClick { get; set; }
        private float TimeFirstClick { get; set; }

        public bool Dragging { get; private set; }

        #endregion


        #region /* Selected Attibutes */

        private SelectionManager Manager { get; set; }
        public Transform Selected { get; private set; }
        private Transform LastClicked { get; set; }

        public Piece Piece { get; private set; }
        public Tile Tile   { get; private set; }

        #endregion


        #region /* Position Attibutes */
        
        private Vector3 PosInScreenSpace { get; set; }
        public Vector3 Offset { get; private set; }

        #endregion



        public SelectionSystem(PuzzleEditor editor, SelectionManager manager, GameController controller)
        {
            this.Editor     = editor;
            this.Manager    = manager;
            this.Controller = controller;

            this.SingleClick = false;

            this.Manager.Initialize(this.Controller);

            this.Editor.Selection = this;
        }



        #region === Info Methods ===

        public void UpdateInfo()
        {
            this.SingleClick = false;

            this.Manager.ReInitialise(this.Controller);
        }


        public bool PieceSelected()
        {
            return this.Piece != null;
        }


        public bool TileSelected()
        {
            return this.Tile != null;
        }


        public bool SomethingSelected()
        {
            return this.Manager.GetCurrentSelection() != null;
        }


        public bool BoardHover()
        {
            return this.Manager.GetBoardHover();
        }

        #endregion


        #region === Manager Methods ===

        public void WhiteListAllItens()
        {
            this.Manager.WhiteList = new List<Transform>();

            foreach (Transform childPiece in this.Editor.PiecesTransform())
            {
                this.Manager.WhiteList.Add(childPiece);
            }

            foreach (Transform childTile in this.Editor.TilesTransform())
            {
                this.Manager.WhiteList.Add(childTile);
            }

        }


        public void EmptyWhiteList()
        {
            this.Manager.WhiteList = new List<Transform>();
        }


        public void AddItemToWhiteList(Transform newItem)
        {
            this.Manager.WhiteList.Add(newItem);
        }


        public void RemoveItemFromWhiteList(Transform item)
        {
            this.Manager.WhiteList.Remove(item);
        }


        public GameObject GameObjectSelected() 
        {
            return this.Manager.GetCurrentSelection().gameObject;
        }


        public Vector2Int BoardCoords()
        {
            return this.Manager.GetBoardCoords();
        }

        public Vector2Int BoardCoordsOffset()
        {
            return this.Manager.GetBoardCoordsOffset();
        }

        #endregion


        #region === Click Methods ===

        public bool DoubleClick()
        {
            if (!this.SingleClick)
            {
                this.TimeFirstClick = Time.time;
                this.SingleClick = true;
                this.LastClicked = this.Manager.GetCurrentSelection();
            }
            else
            {
                Transform clicked = this.Manager.GetCurrentSelection();
                if (((Time.time - this.TimeFirstClick) > DOUBLE_CLICK_WINDOW) ||
                    (this.LastClicked != clicked))
                {
                        this.TimeFirstClick = Time.time;
                        this.LastClicked = clicked;
                }
                else
                {
                    this.SingleClick = false;
                    this.LastClicked = null;
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region === Drag Methods ===

        public void PrepareDrag()
        {
            this.Selected = this.Manager.GetCurrentSelection();

            if (this.Selected != null)
            {
                //this.Dragging = true;
                //this.PosInScreenSpace = Camera.main.WorldToScreenPoint(this.Selected.position);

                //Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, PosInScreenSpace.z);
                //this.Offset = this.Selected.position - Camera.main.ScreenToWorldPoint(newPosition);

                //// one of them is always null
                //this.Piece = this.Selected.GetComponent<Piece>();
                //this.Tile = this.Selected.GetComponent<Tile>();

                ////reset and disable tile temporarily (visual) 
                //if (this.TileSelected())
                //{
                //    this.Tile.enabled = false;
                //}

                this.Editor.RemoveItemSelected();

            }
        }


        public void EndDrag()
        {
            this.Dragging = false;

            if (this.Selected != null)
            {
                Vector2Int newPos = this.Editor.Discretize(this.Selected.position);

                // submits new item's position
                if (this.PieceSelected())
                {
                    this.Editor.MovePiece(newPos, this.Piece);
                }
                else
                {
                    this.Editor.MoveTile(newPos, this.Tile);

                    //re-enable tile  (visual)
                    this.Tile.enabled = true;
                }

                if (!this.BoardHover())
                {
                    this.Editor.RemoveItemSelected();
                }
            }
        }

        #endregion

    }

}
