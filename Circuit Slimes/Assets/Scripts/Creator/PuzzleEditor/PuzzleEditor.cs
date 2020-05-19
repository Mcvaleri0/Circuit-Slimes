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

        List<Transform> ItemsPlaced { get; set; }

        #endregion



        #region === Init Methods ===

        public PuzzleEditor(Puzzle.Puzzle puzzle)
        {
            this.Puzzle = puzzle;
            this.ItemsPlaced = new List<Transform>();
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


        public void RemoveItemToPlace()
        {
            this.ItemButton.Deselect();

            this.Item = null;
            this.ItemButton = null;
        }


        public void PlaceItem()
        {
            if (this.Selection.BoardHover())
            {
                Vector2Int coords = this.Selection.BoardCoords();

                if (this.Item.Contains("Tile"))
                {
                    Tile res = this.Puzzle.CreateTile(Tile.GetType(this.Item), coords);
                    
                    if (res != null)
                    {
                        this.Selection.AddItemToWhiteList(res.transform);
                        this.ItemsPlaced.Add(res.transform);
                    }
                }
                else
                {
                    Piece res = this.Puzzle.CreatePiece(new Piece.Caracteristics(this.Item), coords);

                    if (res != null)
                    {
                        this.Selection.AddItemToWhiteList(res.transform);
                        this.ItemsPlaced.Add(res.transform);
                    }
                }
            }
            else
            {
                this.RemoveItemToPlace();
            }
        }


        public void RemoveItemSelected()
        {
            if (this.Selection.SomethingSelected())
            {
                GameObject objToRemove = this.Selection.GameObjectSelected();
                Vector2Int coords = this.Selection.BoardCoords();

                // remove object representation from Puzzle
                Piece pieceToRemove = this.Puzzle.GetPiece(coords);
                Tile  tileToRemove  = this.Puzzle.GetTile(coords);

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
                    this.Selection.RemoveItemFromWhiteList(objToRemove.transform);
                    this.ItemsPlaced.Remove(objToRemove.transform);
                }
            }
        }


        public void RemoveItemsPlaced()
        {
            Piece piece;
            Tile  tile;
            bool removed;

            List<Transform> itens = new List<Transform>();

            foreach (Transform item in this.ItemsPlaced)
            {
                piece = item.GetComponent<Piece>();
                tile  = item.GetComponent<Tile>();
                removed = false;

                if (piece != null)
                {
                    removed = this.Puzzle.RemovePiece(piece);
                }
                else if (tile != null)
                {
                    removed = this.Puzzle.RemoveTile(tile);
                }

                if (removed)
                {
                    GameObject.Destroy(item.gameObject);
                    this.Selection.RemoveItemFromWhiteList(item);
                }
                else
                {
                    itens.Add(item);
                }
            }

            this.ItemsPlaced = itens;
        }


        public void MoveItem()
        {
            //get board coords
            Vector2Int coords = this.Selection.BoardCoords();

            //get origin offset
            Vector2Int off = this.Selection.BoardCoordsOffset();

            //convert grid coords in world coords
            Vector3 curPosition = this.Puzzle.WorldCoords(coords) + this.Selection.Offset;

            // the new position must be at the board surface
            curPosition = this.Puzzle.AtBoardSurface(curPosition);

            // update the position of the object in the world
            if (this.Selection.PieceSelected())
            {
                if (this.Puzzle.IsFree(coords - off, this.Selection.Piece))
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


        #region === Resources Methods ===

        public List<string> Resources()
        {
            return this.Puzzle.GetAllResources();
        }


        public Resource GetResource(string name)
        {
            return this.Puzzle.GetResource(name);
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
