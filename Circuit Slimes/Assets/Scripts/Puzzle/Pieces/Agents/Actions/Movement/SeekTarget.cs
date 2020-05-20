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

        public SeekTarget(Agent agent, Piece target)
        {
            this.Target = target;

            this.TargetCoords = target.Coords;

            this.TargetCharacteristics = new Piece.Characteristics(target.characterization.ToString());

            int dX = target.Coords.x - agent.Coords.x;
            int dY = target.Coords.y - agent.Coords.y;

            Vector2Int moveCoords = target.Coords - agent.Coords;
            moveCoords.x = (int)Mathf.Sign(moveCoords.x) * Mathf.Min(Mathf.Abs(moveCoords.x), 1);
            moveCoords.y = (int)Mathf.Sign(moveCoords.y) * Mathf.Min(Mathf.Abs(moveCoords.y), 1);

            Vector2Int targetCoords = agent.Coords + moveCoords;

            if (!agent.IsFree(targetCoords) && target.Coords != targetCoords)
            {
                if (Mathf.Abs(dX) > Mathf.Abs(dY))
                {
                    targetCoords.y = agent.Coords.y;
                }
                else
                {
                    targetCoords.x = agent.Coords.x;
                }
            }

            this.MoveCoords = targetCoords;

            this.Direction = LevelBoard.GetDirection(agent.Coords, targetCoords);
            Debug.Log(this.Direction);
        }


        #region Action Methods
        override public Action Available(Agent agent)
        {
            List<Piece> foundPieces = agent.PiecesInSight(3);

            if(foundPieces.Count > 0)
            {
                foreach (Piece piece in foundPieces) {
                    if (this.TargetCharacteristics.Matches(piece.characterization))
                    {
                        return new SeekTarget(agent, piece);
                    }
                }
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