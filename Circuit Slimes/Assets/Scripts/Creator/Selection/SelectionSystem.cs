using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Editor;



namespace Creator.Selection
{
    public class SelectionSystem
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private PuzzleController Controller { get; set; }

        #endregion


        #region /* Click Attributes */

        private const float DOUBLE_CLICK_WINDOW = 0.5f;

        private bool SingleClick { get; set; }
        private float TimeFirstClick { get; set; }

        public bool Dragging { get; private set; }

        #endregion


        #region /* Selected Attibutes */

        private SelectionManager Manager { get; set; }
        public Transform Selected { get; private set; }

        public Piece Piece { get; private set; }
        public Tile Tile   { get; private set; }

        #endregion


        #region /* Position Attibutes */
        
        private Vector3 PosInScreenSpace { get; set; }
        public Vector3 Offset { get; private set; }

        #endregion



        public SelectionSystem(PuzzleEditor editor, SelectionManager manager, PuzzleController controller)
        {
            this.Editor     = editor;
            this.Manager    = manager;
            this.Controller = controller;

            this.SingleClick = false;

            this.Manager.Initialize(this.Controller, this.Editor.PuzzleTransform());

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
            return this.Manager.CurrentSelection != null;
        }


        public bool BoardHover()
        {
            return this.Manager.BoardHover;
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


        public void AddItemToWhiteList(Transform newItem)
        {
            this.Manager.WhiteList.Add(newItem);
        }


        public void EmptyWhiteList()
        {
            this.Manager.WhiteList = new List<Transform>();
        }


        public GameObject GameObjectSelected() 
        {
            return this.Manager.CurrentSelection.gameObject;
        }


        public Vector2Int BoardCoords()
        {
            return this.Manager.BoardCoords;
        }

        #endregion


        #region === Click Methods ===

        public bool DoubleClick()
        {
            if (!this.SingleClick)
            {
                this.TimeFirstClick = Time.time;
                this.SingleClick = true;
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

        #endregion


        #region === Drag Methods ===

        public void PrepareDrag()
        {
            this.Selected = this.Manager.CurrentSelection;

            if (this.Selected != null)
            {
                this.Dragging = true;
                this.PosInScreenSpace = Camera.main.WorldToScreenPoint(this.Selected.position);

                Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, PosInScreenSpace.z);
                this.Offset = this.Selected.position - Camera.main.ScreenToWorldPoint(newPosition);

                // one of them is always null
                this.Piece = this.Selected.GetComponent<Piece>();
                this.Tile = this.Selected.GetComponent<Tile>();

                //reset and disable tile temporarily (visual) 
                if (this.TileSelected())
                {
                    this.Tile.enabled = false;
                }

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
            }
        }

        #endregion

    }

}
