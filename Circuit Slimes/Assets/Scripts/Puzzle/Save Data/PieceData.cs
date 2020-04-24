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
        public string PrefabName;

        public Vector2Int[] Coords;


        public PieceData(Piece piece)
        {
            this.Coords = new Vector2Int[1];
            this.Coords[0] = piece.Coords;

            this.PrefabName = piece.Caracterization.ToString();
        }

        public PieceData(Piece[] pieces)
        {
            this.Coords = new Vector2Int[pieces.Length];

            var i = 0;
            foreach(var piece in pieces)
            {
                this.Coords[i++] = piece.Coords;
            }

            this.PrefabName = pieces[0].Caracterization.ToString();
        }


        public Piece[] CreatePieces(Puzzle puzzle)
        {
            var pieces = new Piece[this.Coords.Length];

            var i = 0;
            foreach(var coords in this.Coords)
            {
                pieces[i] = Piece.CreatePiece(puzzle, this.Coords[i], this.PrefabName, default, default);
                i++;
            }

            return pieces;
        }

    }
}
