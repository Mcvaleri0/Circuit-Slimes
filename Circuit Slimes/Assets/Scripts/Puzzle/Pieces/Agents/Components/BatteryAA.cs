using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class BatteryAA : Battery
    {
        protected override void Update()
        {
            base.Update();

            if(Input.GetKeyUp(KeyCode.O))
            {
                var newOri = (LevelBoard.Directions) (((int) this.Orientation + 2) % 8);

                this.Rotate(newOri);
            }
        }


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

        override public Vector2Int[] GetFootprintAt(Vector2Int coords, LevelBoard.Directions orientation)
        {
            Vector2Int[] footprint = new Vector2Int[2]
            {
                this.Coords,
                this.Coords + LevelBoard.DirectionalVectors[(int) orientation]
            };

            return footprint;
        }

        new public bool Rotate(LevelBoard.Directions targetDir)
        {
            return this.Board.RotatePiece(this, targetDir);
        }
        #endregion
    }
}