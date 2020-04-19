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


        public override Action Available(Agent agent)
        {
            return null;
        }


        public override bool Execute(Agent agent)
        {
            if(agent.Rotate(this.Direction))
            {
                return agent.Move(this.TargetCoords);
            }
            
            return false;
        }
    }
}