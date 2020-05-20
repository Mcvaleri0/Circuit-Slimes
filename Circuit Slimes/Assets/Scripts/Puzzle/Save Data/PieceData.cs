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

        public LevelBoard.Directions[] Orientations;


        public PieceData(Piece piece)
        {
            this.Coords = new Vector2Int[1];
            this.Coords[0] = piece.Coords;

            this.Orientations = new LevelBoard.Directions[1];
            this.Orientations[0] = piece.Orientation;

            this.PrefabName = piece.characterization.ToString();
        }

        public PieceData(Piece[] pieces)
        {
            this.Coords = new Vector2Int[pieces.Length];
            this.Orientations = new LevelBoard.Directions[pieces.Length];

            var i = 0;
            foreach(var piece in pieces)
            {
                this.Coords[i] = piece.Coords;
                this.Orientations[i++] = piece.Orientation;
            }

            this.PrefabName = pieces[0].characterization.ToString();
        }


        public Piece[] CreatePieces(Puzzle puzzle)
        {
            var pieces = new Piece[this.Coords.Length];

            var i = 0;
            foreach(var coords in this.Coords)
            {
                pieces[i] = Piece.CreatePiece(puzzle, this.PrefabName, this.Coords[i], this.Orientations[i], default);
                i++;
            }

            return pieces;
        }

    }
}
