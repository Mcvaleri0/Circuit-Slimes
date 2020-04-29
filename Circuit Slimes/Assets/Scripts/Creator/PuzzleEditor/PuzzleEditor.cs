using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Puzzle;
using Creator.Selection;
using Creator.UI.Buttons;



namespace Creator.Editor
{
    public class PuzzleEditor
    {
        #region /* Creator Sub-Components */

        public SelectionSystem Selection { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        private string Item { get; set; }
        private OptionButton ItemButton { get; set; }

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
            return this.Item != null;
        }


        public void ItemToPlace(string itemName, OptionButton itemButton)
        {
            if (this.HasItemToPlace())
            {
                this.ItemButton.Deselect();
            }

            this.Item = itemName;
            this.ItemButton = itemButton;
        }


        private void NoItemToPlace()
        {
            this.Item = null;

            this.ItemButton.Deselect();
            this.ItemButton = null;
        }


        public void PlaceItem()
        {
            if (this.Selection.BoardHover())
            {
                Vector2Int coords = this.Selection.BoardCoords();

                if (this.Item.Contains("Tile"))
                {
                    Tile newTile = Tile.CreateTile(this.Puzzle, coords, this.Item);

                    if (this.Puzzle.IsFree(coords, newTile) && this.Puzzle.AddTile(newTile))
                    {
                        this.Selection.AddItemToWhiteList(newTile.transform);
                    }
                    else
                    {
                        GameObject.Destroy(newTile.gameObject);
                    }
                }
                else
                {
                    Piece newPiece = Piece.CreatePiece(this.Puzzle, coords, this.Item);

                    if (this.Puzzle.IsFree(coords, newPiece) && this.Puzzle.AddPiece(newPiece))
                    {
                        this.Selection.AddItemToWhiteList(newPiece.transform);
                    }
                    else
                    {
                        GameObject.Destroy(newPiece.gameObject);
                    }
                }
            }
            else
            {
                this.NoItemToPlace();
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

                bool removed = false;
                if (pieceToRemove != null)
                {
                    removed = this.Puzzle.RemovePiece(pieceToRemove);
                }
                else if (tileToRemove != null)
                {
                    removed = this.Puzzle.RemoveTile(tileToRemove);
                }

                if (removed)
                {
                    GameObject.Destroy(objToRemove);
                }
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
                if (this.Puzzle.IsFree(coords, this.Selection.Piece))
                {
                    this.ChangeItemPosition(this.Selection.Selected, curPosition);
                }
            }
            else if (this.Selection.TileSelected())
            {
                if (this.Puzzle.IsFree(coords, this.Selection.Tile))
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
            bool success = this.Puzzle.MovePiece(newPos, piece);

            if (!success)
            {
                Transform pieceTransform = piece.transform;
                Vector3 oldPos = this.Puzzle.WorldCoords(piece.Coords);

                this.ChangeItemPosition(pieceTransform, oldPos);
            }
        }

        #endregion


        #region === Tiles Methods ===

        public Transform TilesTransform()
        {
            return this.Puzzle.TilesObj.transform;
        }


        public void MoveTile(Vector2Int newPos, Tile tile)
        {
            bool success = this.Puzzle.MoveTile(newPos, tile);

            if (!success)
            {
                Transform pieceTransform = tile.transform;
                Vector3 oldPos = this.Puzzle.WorldCoords(tile.Coords);

                this.ChangeItemPosition(pieceTransform, oldPos);
            }

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
