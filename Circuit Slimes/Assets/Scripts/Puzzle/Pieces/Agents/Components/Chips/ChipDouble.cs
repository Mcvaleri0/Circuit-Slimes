using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class ChipDouble : Chip
    {
        //footprint
        public override Vector2Int[] Footprint { get; set; } = {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0)
        };

        new public bool Rotate(LevelBoard.Directions targetDir)
        {
            return this.Board.RotatePiece(this, targetDir);
        }
    }
}
