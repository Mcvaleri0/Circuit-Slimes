using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle
{
    public class Puzzle : MonoBehaviour
    {
        #region /* Attributes */

        public List<Piece> Pieces { get; private set; }

        public List<Agent> Agents { get; private set; }

        public LevelBoard Board { get; private set; }

        public GameObject PiecesObj { get; private set; }

        public GameObject TilesObj { get; private set; }

        public List<string> Permissions { get; private set; }

        public WinCondition WinCondition { get; private set; }
        #endregion


        #region === Initialization Methods ===

        public void Initialize(LevelBoard board)
        {
            Initialize(board, new List<Piece>(), new List<Tile>(), new List<string>(), null);
        }

        public void Initialize(LevelBoard board, List<Piece> pieces, List<Tile> tiles, List<string> permissions, WinCondition winCondition)
        {
            this.Board  = board;
            this.Pieces = pieces;
            this.Permissions = permissions;
            this.WinCondition = winCondition;

            this.Agents = new List<Agent>();

            foreach(var piece in pieces)
            {
                this.Board.PlacePiece(piece.Coords, piece);
            }

            this.Agents = this.Board.GetAllAgents();

            foreach (var tile in tiles)
            {
                this.Board.PlaceTile(tile.Coords, tile);
            }

            this.PiecesObj = this.transform.GetChild(0).gameObject;
            this.TilesObj  = this.transform.GetChild(1).gameObject;
        }

        public void Destroy()
        {
            GameObject.Destroy(this.gameObject);
        }

        #endregion


        #region === Piece Methods ===

        public bool AddPiece(Piece piece)
        {
            if (!this.Board.PlacePiece(piece.Coords, piece)) return false;

            this.Pieces.Add(piece);

            if (piece is Agent agent) this.Agents.Add(agent);

            if (piece.transform.parent == null)
                piece.transform.parent = this.PiecesObj.transform;

            return true;
        }


        public bool RemovePiece(Piece piece)
        {
            // Remove the Piece from the Puzzle's Piece List
            if (!this.Pieces.Remove(piece)) return false;
            
            // If it's an Agent, remove it from the Puzzle's Agent List
            if (piece is Agent agent) this.Agents.Remove(agent);

            // Remove the Piece from the Board
            if (!this.Board.RemovePieceAt(piece.Coords)) return false;

            return true;
        }


        public bool MovePiece(Vector2Int coords, Piece piece)
        {
            return this.Board.MovePiece(coords, piece);
        }


        public Piece GetPiece(Vector2Int coords)
        {
            return this.Board.GetPiece(coords);
        }


        public bool IsFree(Vector2Int coords, Piece piece) 
        {
            return this.Board.CanPlacePiece(coords, piece);
        }

        #endregion


        #region === Tile Methods ===

        public bool AddTile(Tile tile)
        {
            bool success = this.Board.PlaceTile(tile.Coords, tile);

            if (tile.transform.parent == null)
                tile.transform.parent = this.TilesObj.transform;

            return success;
        }


        public bool RemoveTile(Tile tile)
        {
            return this.Board.RemoveTileAt(tile.Coords) != null;
        }


        public bool MoveTile(Vector2Int coords, Tile tile)
        {
            return this.Board.MoveTile(coords, tile);
        }


        public Tile GetTile(Vector2Int coords)
        {
            return this.Board.GetTile(coords);
        }


        public void UpdateAllTiles()
        {
            this.Board.UpdateAllTiles();
        }


        public bool IsFree(Vector2Int coords, Tile tile)
        {
            Tile tileNewPos = this.GetTile(coords);

            if (tileNewPos == null || tileNewPos == tile)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion


        #region === Permission Methods ===

        public void AddPermission(string prefab)
        {
            this.Permissions.Add(prefab);
        }

        public void RemovePermission(string prefab)
        {
            this.Permissions.Remove(prefab);
        }

        #endregion


        #region === Utility ===

        public bool OutOfBounds(Vector2Int coords)
        {
            return this.Board.OutOfBounds(coords);
        }

        public Vector3 AtBoardSurface(Vector3 coords)
        {
            return this.Board.AtBoardSurface(coords);
        }

        public Vector2Int Discretize(Vector3 position)
        {
            return LevelBoard.Discretize(position);
        }

        public Vector3 WorldCoords(Vector2Int position)
        {
            return LevelBoard.WorldCoords(position);
        }

        public Vector2Int Clamp(Vector2Int coords)
        {
            int x = Mathf.Clamp(coords.x, 0, Board.Width - 1);
            int y = Mathf.Clamp(coords.y, 0, Board.Height - 1);

            return new Vector2Int(x, y);
        }

        #endregion
    }
}
