using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;


namespace Puzzle
{
    public class Puzzle : MonoBehaviour
    {
        public List<Piece> Pieces { get; private set; }

        public LevelBoard Board { get; private set; }

        public GameObject PuzzleObj { get; private set; }


        public void Initialize(LevelBoard board)
        {
            Initialize(board, new List<Piece>(), new List<Tile>());
        }

        public void Initialize(LevelBoard board, List<Piece> pieces, List<Tile> tiles)
        {
            this.Board  = board;
            this.Pieces = pieces;

            foreach(var piece in pieces)
            {
                this.Board.PlacePiece((int) piece.Coords.x, (int) piece.Coords.y, piece);
            }

            foreach (var tile in tiles)
            {
                this.Board.PlaceTile((int)tile.Coords.x, (int)tile.Coords.y, tile);
            }
        }

        public void AddPiece(Piece piece)
        {
            this.Pieces.Add(piece);
            this.Board.PlacePiece((int)piece.Coords.x, (int)piece.Coords.y, piece);
        }

        public void AddTile(Tile tile)
        {
            this.Board.PlaceTile((int)tile.Coords.x, (int)tile.Coords.y, tile);
        }

        public void Destroy()
        {
            GameObject.Destroy(this.PuzzleObj);

            foreach(var piece in this.Pieces)
            {
                GameObject.Destroy(piece.gameObject);
            }

            this.Pieces.Clear();

            GameObject.Destroy(this.Board.Plane);
            this.Board = null;

            GameObject.Destroy(this.gameObject);
        }
    }
}
