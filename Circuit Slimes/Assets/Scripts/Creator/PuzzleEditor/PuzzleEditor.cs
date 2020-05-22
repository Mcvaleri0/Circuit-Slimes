using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        
        public void PlaceItem(Resource resource)
        {
            if ((this.Selection.BoardHover()) && (resource.Available()))
            {
                Vector2Int coords = this.Selection.BoardCoords();

                if (resource.isTile())
                {
                    Tile res = this.Puzzle.CreateTile(Tile.GetType(resource.Name), coords);
                    
                    if (res != null)
                    {
                        this.Selection.AddItemToWhiteList(res.transform);
                        this.ItemsPlaced.Add(res.transform);
                        resource.Decrease();
                    }
                }
                else
                {
                    Piece res = this.Puzzle.CreatePiece(new Piece.Characteristics(resource.Name), coords);

                    if (res != null)
                    {
                        this.Selection.AddItemToWhiteList(res.transform);
                        this.ItemsPlaced.Add(res.transform);
                        resource.Decrease();
                    }
                }
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

                    Resource resource = this.GetResource(objToRemove.name);
                    resource.Increase();
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
