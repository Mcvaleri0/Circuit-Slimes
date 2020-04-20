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

        public GameObject PuzzleObj { get; private set; }

        #endregion


        #region === Initialization Methods ===

        public void Initialize(LevelBoard board)
        {
            Initialize(board, new List<Piece>(), new List<Tile>());
        }

        public void Initialize(LevelBoard board, List<Piece> pieces, List<Tile> tiles)
        {
            this.Board  = board;
            this.Pieces = pieces;

            this.Agents = new List<Agent>();

            foreach(var piece in pieces)
            {
                this.Board.PlacePiece(piece.Coords, piece);

                if (piece is Agent agent) this.Agents.Add(agent);
            }

            foreach (var tile in tiles)
            {
                this.Board.PlaceTile(tile.Coords, tile);
            }
        }

        public void Destroy()
        {
            foreach (var piece in this.Pieces)
            {
                GameObject.Destroy(piece.gameObject);
            }

            this.Pieces.Clear();

            GameObject.Destroy(this.Board.gameObject);

            GameObject.Destroy(this.gameObject);
        }

        #endregion


        #region === Piece Methods ===

        public void AddPiece(Piece piece)
        {
            this.Pieces.Add(piece);
            if (piece is Agent agent) this.Agents.Add(agent);
            this.Board.PlacePiece(piece.Coords, piece);
        }

        public void RemovePiece(Piece piece)
        {
            this.Pieces.Remove(piece);
            if (piece is Agent agent) this.Agents.Remove(agent);
            this.Board.RemovePiece(piece.Coords);
        }

        public bool MovePiece(Vector2Int coords, Piece piece)
        {
            return this.Board.MovePiece(coords, piece);
        }

        public Piece GetPiece(Vector2Int coords)
        {
            return this.Board.GetPiece(coords);
        }

        #endregion


        #region === Tile Methods ===

        public void AddTile(Tile tile)
        {
            this.Board.PlaceTile(tile.Coords, tile);
        }

        public void RemoveTile(Tile tile)
        {
            this.Board.RemoveTile(tile.Coords);
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

        #endregion


        #region === Utility ===

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

        #endregion
    }
}
