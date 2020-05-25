using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Puzzle;
using Creator.UI.Drawer;
using Creator.Selection;
using Puzzle.Board;

namespace Creator.Editor
{
    public class PuzzleEditor
    {
        #region /* Creator Sub-Components */

        private CreatorController Controller { get; set; }
        public SelectionSystem Selection { get; set; }
        public Mode.Mode Mode { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        public Puzzle.Puzzle Puzzle { get; private set; }

        private List<Transform> ItemsPlaced { get; set; }

        #endregion


        #region /* Moving Item Attributes */

        private Transform MovingSprite { get; set; }
        private Vector2Int MovigStartPos { get; set; }
        
        private Option MovingOption { get; set; }
        private Draggable MovingDrag { get; set; }
        private Text MovingText { get; set; }
        private LevelBoard.Directions direction { get; set; }

        #endregion



        #region === Init Methods ===

        public PuzzleEditor(CreatorController Controller, Puzzle.Puzzle puzzle)
        {
            this.Controller = Controller;
            this.Puzzle = puzzle;
            this.ItemsPlaced = new List<Transform>();

            this.InitializeMovingItem();
        }


        private void InitializeMovingItem()
        {
            Object prefab = UnityEngine.Resources.Load(FileHelper.OPTION_PATH);

            GameObject item = Option.CreateOption(this, null, prefab, this.Controller.transform.Find("Canvas"), "", false);
            item.name = "MovingItem";

            Transform resource = item.transform.Find("Resource");
            this.MovingSprite = resource.Find("Sprite");
            this.MovingOption = item.GetComponent<Option>();
            this.MovingDrag = this.MovingSprite.GetComponent<Draggable>();
            this.MovingText = resource.Find("Amount").GetComponent<Text>();
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
            if (this.Selection.BoardHover())
            {
                Vector2Int coords = this.Selection.BoardCoords();

                if (!this.PlaceResource(resource, coords) && this.Selection.Dragging)
                {
                    this.PlaceResource(resource, this.MovigStartPos);
                }
            }
        }


        private bool PlaceResource(Resource resource, Vector2Int coords)
        {
            if (resource.isTile())
            {
                Tile res = this.Puzzle.CreateTile(Tile.GetType(resource.Name), coords);

                if (res != null)
                {
                    this.Selection.AddItemToWhiteList(res.transform);
                    this.ItemsPlaced.Add(res.transform);
                    resource.Decrease();

                    return true;
                }
            }
            else
            {
                Piece res = this.Puzzle.CreatePiece(new Piece.Characteristics(resource.Name), coords, this.direction);

                if (res != null)
                {
                    this.Selection.AddItemToWhiteList(res.transform);
                    this.ItemsPlaced.Add(res.transform);
                    resource.Decrease();

                    return true;
                }
            }

            return false;
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

                    Resource resource = this.GetResource(item.name);
                    resource.Increase();
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


        public void RotateItem()
        {
            if (this.Selection.SomethingSelected())
            {
                if (this.Selection.PieceSelected())
                {
                    this.Puzzle.RotatePieceRight(this.Selection.Piece);
                }
            }
        }


        private void ChangeItemPosition(Transform item, Vector3 newPos)
        {
            item.position = newPos;
        }


        public void InitializeMovingItem(Transform selected)
        {
            Option.InitiliazeSprite(this.MovingSprite, selected.name);
            this.MovigStartPos = this.Puzzle.Discretize(selected.position);
            this.MovingOption.Initialize(this, selected.name, this.MovingText, this.MovingDrag, this.Mode.AbleToEditOptions());
            this.MovingDrag.InitializeDrag();

            Piece piece = selected.GetComponentInChildren<Piece>();
            if (piece != null)
            {
                this.direction = piece.Orientation;
            }
        }


        public void PlaceMovingItem()
        {
            this.MovingDrag.EndDrag();
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
