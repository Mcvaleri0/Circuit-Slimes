using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle
{
    public class Puzzle : MonoBehaviour
    {
        public List<Piece> Pieces { get; private set; }

        public List<Agent> Agents { get; private set; }

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

        public void AddPiece(Piece piece)
        {
            this.Pieces.Add(piece);
            if (piece is Agent agent) this.Agents.Add(agent);
            this.Board.PlacePiece(piece.Coords, piece);
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

            GameObject.Destroy(this.Board.gameObject);

            GameObject.Destroy(this.gameObject);
        }
    }
}
