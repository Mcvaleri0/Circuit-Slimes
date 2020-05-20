using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class SeekTarget : Move
    {
        public Piece Target { get; protected set; }

        public Vector2Int TargetCoords { get; protected set; }

        protected Piece.Characteristics TargetCharacteristics;


        protected SeekTarget(Piece.Characteristics Characteristics) : base()
        {
            this.TargetCharacteristics = Characteristics;
        }

        public SeekTarget(Vector2Int moveCoords, LevelBoard.Directions direction, Piece target)
        {
            this.Direction = direction;

            this.Target = target;

            this.TargetCoords = target.Coords;

            this.TargetCharacteristics = new Piece.Characteristics(target.Characterization.ToString());

            this.MoveCoords = moveCoords;
        }

        #region Action Methods
        override public Action Available(Agent agent)
        {
            List<Vector2Int> pathToTarget = agent.PathToNearest(this.TargetCharacteristics, 3);

            if(pathToTarget != null)
            {
                var dir = LevelBoard.GetDirection(pathToTarget[0], pathToTarget[1]);

                var target = agent.PieceAt(pathToTarget[pathToTarget.Count - 1]);

                return new SeekTarget(pathToTarget[1], dir, target);
            }

            return null;
        }

        override public bool Confirm(Agent agent)
        {
            if(agent.PieceExists(this.Target))
            {
                return base.Confirm(agent);
            }

            return false;
        }
        #endregion
    }
}