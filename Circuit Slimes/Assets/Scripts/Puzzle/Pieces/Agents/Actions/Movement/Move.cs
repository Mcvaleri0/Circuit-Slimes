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
            return agent.Board.CanPlacePiece(this.MoveCoords, this.Direction, agent);
        }

        override public bool Execute(Agent agent)
        {
            var rotated = agent.Rotate(this.Direction);

            var moved = agent.Move(this.MoveCoords);
                        
            return rotated && moved;
        }

        public override bool Undo(Agent agent)
        {
            agent.Rotate(this.Direction, 1f);

            var oppositeDir = (LevelBoard.Directions) (((int) this.Direction + 4) % 8);
            var origCoords = agent.Board.GetAdjacentCoords(this.MoveCoords, oppositeDir);
            
            agent.transform.position = LevelBoard.WorldCoords(origCoords);
            agent.Board.MovePiece(origCoords, agent);

            return base.Undo(agent);
        }
    }
}