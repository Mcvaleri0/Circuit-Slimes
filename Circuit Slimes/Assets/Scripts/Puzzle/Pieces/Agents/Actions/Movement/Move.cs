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

        public Vector2Int TargetCoords { get; protected set; }


        public Move()
        {
            this.Direction = LevelBoard.Directions.None;
        }

        public Move(LevelBoard.Directions dir, Vector2Int coords)
        {
            this.Direction = dir;

            this.TargetCoords = coords;
        }


        override public bool Execute(Agent agent)
        {
            if(agent.Rotate(this.Direction))
            {
                return agent.Move(this.TargetCoords);
            }
            
            return false;
        }

        public override bool Undo(Agent agent)
        {
            agent.Rotate(this.Direction, 1f);

            var oppositeDir = (LevelBoard.Directions) (((int) this.Direction + 4) % 8);
            var origCoords = agent.Board.GetAdjacentCoords(this.TargetCoords, oppositeDir);
            
            agent.transform.position = LevelBoard.WorldCoords(origCoords);
            agent.Board.MovePiece(origCoords, agent);

            return base.Undo(agent);
        }
    }
}