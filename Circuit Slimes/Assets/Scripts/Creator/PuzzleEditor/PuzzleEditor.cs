using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Selection;



namespace Creator.Editor
{
    public class PuzzleEditor
    {
        #region /* Puzzle Attributes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        #endregion


        #region /* Selection Attibutes */

        public SelectionSystem Selection { get; set; }

        #endregion



        #region === Init Methods ===

        public PuzzleEditor(Puzzle.Puzzle puzzle)
        {
            this.Puzzle = puzzle;
        }

        #endregion


        #region === Puzzle Methods ===

        public Transform PuzzleTransform()
        {
            return this.Puzzle.transform;
        }

        
        public void MoveItem()
        {
            //get board coords
            Vector2Int coords = this.Selection.BoardCoords();

            //convert grid coords in world coords
            Vector3 curPosition = this.Puzzle.WorldCoords(coords) + this.Selection.Offset;

            // the new position must be at the board surface
            curPosition = this.Puzzle.AtBoardSurface(curPosition);

            // update the position of the object in the world
            if (this.Selection.PieceSelected())
            {
                Piece pieceNewPos = this.Puzzle.GetPiece(coords);

                if (pieceNewPos == null || pieceNewPos == this.Selection.Piece)
                {
                    this.ChangeItemPosition(this.Selection.Selected, curPosition);
                }
            }
            else
            {
                Tile tileNewPos = this.Puzzle.GetTile(coords);

                if (tileNewPos == null || tileNewPos == this.Selection.Tile)
                {
                    this.ChangeItemPosition(this.Selection.Selected, curPosition);
                }
            }
        }


        public void ChangeItemPosition(Transform item, Vector3 newPos)
        {
            item.position = newPos;
        }


        #endregion


        #region === Pieces Methods ===

        public Transform PiecesTransform()
        {
            return this.Puzzle.PiecesObj.transform;
        }


        public void MovePiece(Vector2Int newPos, Piece piece)
        {
            this.Puzzle.MovePiece(newPos, piece);
        }

        #endregion


        #region === Tiles Methods ===

        public Transform TilesTransform()
        {
            return this.Puzzle.TilesObj.transform;
        }


        public void MoveTile(Vector2Int newPos, Tile tile)
        {
            this.Puzzle.MoveTile(newPos, tile);
        }

        #endregion


        #region === Auxiliary Functions ===

        public Vector2Int Discretize(Vector3 pos)
        {
            return this.Puzzle.Discretize(pos);
        }

        #endregion

    }

}
