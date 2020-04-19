using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Data
{
    /// <summary>
    /// Piece's data that are relevant to be saved
    /// </summary>
    [System.Serializable]
    public class PieceData
    {
        public Vector2Int Coords;

        public Piece.Categories Category;

        public Piece.SlimeTypes SlimeType;

        public Piece.ComponentTypes ComponentType;

        public Piece.CandyTypes CandyType;

        public PieceData(Piece piece)
        {
            this.Coords = piece.Coords;

            this.Category = piece.Category;

            this.SlimeType = piece.SlimeType;

            this.ComponentType = piece.ComponentType;

            this.CandyType = piece.CandyType;
        }


        public Piece CreatePiece(Transform parent, Puzzle puzzle)
        {
            return Piece.CreatePiece(parent, puzzle, this.Coords, this.Category, this.SlimeType, this.ComponentType, this.CandyType);
        }

    }
}
