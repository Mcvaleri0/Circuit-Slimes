using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;

namespace Puzzle.Pieces.Components
{
    public class BatteryAA : Battery
    {
        //footprint
        public override Vector2Int[] Footprint { get; set; } = {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0)
        };

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

        new public bool Rotate(LevelBoard.Directions targetDir)
        {
            return this.Puzzle.RotatePiece(this, targetDir);
        }
        #endregion
    }
}