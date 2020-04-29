using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class BatteryAA : Battery
    {
        #region === Piece Methods ===
        override public Vector2Int[] GetFootprint()
        {
            Vector2Int[] footprint = new Vector2Int[2]
            {
                this.Coords,
                this.Coords + LevelBoard.DirectionalVectors[(int) this.Orientation]
            };

            return footprint;
        }
        #endregion
    }
}