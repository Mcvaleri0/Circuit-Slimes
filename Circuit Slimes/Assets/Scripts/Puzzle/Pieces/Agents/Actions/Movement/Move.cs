using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;
using Puzzle.Board;

namespace Puzzle.Actions
{
    public class Move : Action
    {
        public LevelBoard.Directions Direction { get; protected set; }

        public Vector2Int MoveCoords { get; protected set; }

        protected LevelBoard.Directions OrigOrientation { get; set; }


        public Move()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public Move(LevelBoard.Directions dir, Vector2Int coords)
        {
            this.Direction = dir;

            this.MoveCoords = coords;
        }

        public override bool Confirm(Agent agent)
        {
            // If the Agent can be moved to the new Coords
            if(agent.CanMove(this.MoveCoords))
            {
                this.OrigOrientation = agent.Orientation; // Save current Orientation

                // Rotate the Agent in the Board
                if (agent.RotateInBoard(this.Direction))
                {
                    // Move the Agent in the Board
                    if (agent.MoveInBoard(this.MoveCoords))
                    {
                        return true;
                    }
                    // If it fails, Undo the rotation
                    else
                    {
                        agent.RotateInBoard(this.OrigOrientation);
                    }
                }
            }

            return false;
        }

        override public bool Execute(Agent agent)
        {
            var rotated = agent.Rotate(this.Direction);
            var moved   = agent.Move(this.MoveCoords);
                        
            return rotated && moved;
        }

        public override bool Undo(Agent agent)
        {
            var oppositeDir = (LevelBoard.Directions) (((int) this.Direction + 4) % 8);
            var origCoords  = LevelBoard.GetAdjacentCoords(this.MoveCoords, oppositeDir);
            
            agent.Move(origCoords, 1000f);
            agent.Rotate(this.OrigOrientation, 1f);

            agent.MoveInBoard(origCoords);
            agent.RotateInBoard(this.OrigOrientation);

            return base.Undo(agent);
        }
    }
}