using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle;
using Creator.Selection;



namespace Creator.Editor
{
    public class PuzzleEditor
    {
        #region /* Creator Sub-Components */

        public SelectionSystem Selection { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        public string ItemToPlace { get; set; }

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


        public void UpdatePuzzle(Puzzle.Puzzle puzzle)
        {
            this.Puzzle = puzzle;
        }

        #endregion


        #region === Items Methods ===

        public bool HasItemToPlace()
        {
            return this.ItemToPlace != null;
        }


        public void PlaceItem()
        {
            Debug.Log("Instantiating " + this.ItemToPlace);

            // TODO: make the player choose where he wants the item
            Vector2Int coords = this.Selection.BoardCoords();
            //Vector2Int coords = new Vector2Int(0, 3);

            if (this.ItemToPlace.Contains("Tile"))
            {
                Tile newTile = Tile.CreateTile(this.Puzzle, coords, this.ItemToPlace);
                this.Puzzle.AddTile(newTile);
                this.Selection.AddItemToWhiteList(newTile.transform);
            }
            else
            {
                Piece newPiece = Piece.CreatePiece(this.Puzzle, coords, this.ItemToPlace);
                this.Puzzle.AddPiece(newPiece);
                this.Selection.AddItemToWhiteList(newPiece.transform);
            }
        }


        public void RemoveItem()
        {
            if (this.Selection.SomethingSelected())
            {
                GameObject objToRemove = this.Selection.GameObjectSelected();
                Vector2Int coords = this.Selection.BoardCoords();

                // remove object representation from Puzzle
                Piece pieceToRemove = this.Puzzle.GetPiece(coords);
                Tile tileToRemove = this.Puzzle.GetTile(coords);

                if (pieceToRemove != null)
                {
                    this.Puzzle.RemovePiece(pieceToRemove);
                }
                else if (tileToRemove != null)
                {
                    this.Puzzle.RemoveTile(tileToRemove);
                }

                GameObject.Destroy(objToRemove);
            }
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
            else if (this.Selection.TileSelected())
            {
                Tile tileNewPos = this.Puzzle.GetTile(coords);

                if (tileNewPos == null || tileNewPos == this.Selection.Tile)
                {
                    this.ChangeItemPosition(this.Selection.Selected, curPosition);
                }
            }
        }


        private void ChangeItemPosition(Transform item, Vector3 newPos)
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


        #region === Permissions Methods ===

        public List<string> Permissions()
        {
            return this.Puzzle.Permissions;
        }

        public void AddPermission(string prefab)
        {
            this.Puzzle.AddPermission(prefab);
        }


        public void RemovePermission(string prefab)
        {
            this.Puzzle.RemovePermission(prefab);
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
