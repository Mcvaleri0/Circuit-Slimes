using UnityEngine;
using System.Collections.Generic;
using Puzzle.Pieces.Components;

namespace Puzzle
{
    public class WinCondition
    {
        public enum Conditions
        {
            None,
            GLED_On_RLED_Off
        }

        public readonly Conditions Condition = 0;

        public WinCondition(Conditions condition)
        {
            this.Condition = condition;
        }

        public bool Verify(Puzzle puzzle)
        {
            switch(this.Condition)
            {
                default:
                    return false;

                case Conditions.GLED_On_RLED_Off:
                    return this.CheckLEDs(puzzle);
            }
        }


        private bool CheckLEDs(Puzzle puzzle)
        {            
            foreach(Piece piece in puzzle.Pieces)
            {
                if(piece is LED led)
                {
                    if((led.Characterization.ComponentType == Piece.ComponentTypes.GreenLED &&
                       led.Stats.Food < led.Stats.MaxFood) ||
                       (led.Characterization.ComponentType == Piece.ComponentTypes.RedLED &&
                       led.Stats.Food >= led.Stats.MaxFood))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}