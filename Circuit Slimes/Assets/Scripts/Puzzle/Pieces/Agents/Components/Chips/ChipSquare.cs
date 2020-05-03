using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class ChipSquare : Chip
    {
        //footprint
        override public Vector2Int[] GetFootprint()
        {
            Vector2Int[] footprint = new Vector2Int[4]
            {
                this.Coords,
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.East],
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.SouthEast],
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.South]
            };

            return footprint;
        }

        override public Vector2Int[] GetFootprintAt(Vector2Int coords, LevelBoard.Directions orientation)
        {
            Vector2Int[] footprint = new Vector2Int[4]
            {
                this.Coords,
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.East],
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.SouthEast],
                this.Coords + LevelBoard.DirectionalVectors[(int) LevelBoard.Directions.South],
            };

            return footprint;
        }

        new public bool Rotate(LevelBoard.Directions targetDir)
        {
            return this.Board.RotatePiece(this, targetDir);
        }
    }
}
