using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Editor;



namespace Creator.Selection
{
    public class SelectionSystem
    {
        #region /* Click Attributes */

        private const float DOUBLE_CLICK_WINDOW = 0.5f;

        private bool SingleClick { get; set; }
        private float TimeFirstClick { get; set; }

        public bool MouseHolded { get; private set; }

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



        public SelectionSystem(SelectionManager manager, PuzzleController controller, Transform puzzle)
        {
            this.Manager = manager;

            this.SingleClick = false;

            this.Manager.Initialize(controller, puzzle);
        }



        #region === Info Methods ===

        public void UpdateInfo(PuzzleController controller)
        {
            this.SingleClick = false;

            this.Manager.ReInitialise(controller);
        }


        public bool PieceSelected()
        {
            return this.Piece != null;
        }


        public bool TileSelected()
        {
            return this.Tile != null;
        }

        #endregion


        #region === Manager Methods ===

        public void WhiteListAllItens(Transform pieces, Transform tiles)
        {
            this.Manager.WhiteList = new List<Transform>();

            foreach (Transform childPiece in pieces)
            {
                this.Manager.WhiteList.Add(childPiece);
            }

            foreach (Transform childTile in tiles)
            {
                this.Manager.WhiteList.Add(childTile);
            }

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
                this.MouseHolded = true;
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


        public void EndDrag(PuzzleEditor editor)
        {
            this.MouseHolded = false;

            if (this.Selected != null)
            {
                Vector2Int newPos = editor.Discretize(this.Selected.position);

                if (this.Piece != null)
                {
                    editor.MovePiece(newPos, this.Piece);
                }
                else
                {
                    editor.MoveTile(newPos, this.Tile);

                    //re-enable tile  (visual)
                    this.Tile.enabled = true;
                }
            }
        }

        #endregion

    }

}
